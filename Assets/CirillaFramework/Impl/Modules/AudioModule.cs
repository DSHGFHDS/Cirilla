using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class AudioModule : IAudioModule
    {
        private List<GameObject> audioPool;
        private Queue<GameObject> validPool;
        private Dictionary<int, AudioInfo> channels;
        private GameObject audioPoolGo;
        public AudioModule()
        {
            audioPool = new List<GameObject>();
            validPool = new Queue<GameObject>();
            channels = new Dictionary<int, AudioInfo>();
            Core.AttachToCirilla(audioPoolGo = new GameObject("AudioPool"));
        }
        
        public AudioChannel Play(AudioClip audioClip, bool loop, int index) => Play(null, null, audioClip, loop, index);
        public AudioChannel Play(Vector3 pos, AudioClip audioClip, bool loop, int index) => Play(null, pos, audioClip, loop, index);
        public AudioChannel Play(Transform ownerTrans, AudioClip audioClip, bool loop, int index) => Play(ownerTrans, null, audioClip, loop, index);

        public void Clear()
        {
            foreach(AudioInfo audioInfo in new Dictionary<int, AudioInfo>(channels).Values)
                audioInfo.stop();

            audioPool.ForEach((go) => GameObject.Destroy(go));
            audioPool.Clear();
            validPool.Clear();
        }

        private AudioChannel Play(Transform ownerTrans, Vector3? pos, AudioClip audioClip, bool loop, int index)
        {
            if (audioClip == null)
                return null;

            if (index == 0)
            {
                do { index = Guid.NewGuid().ToString().GetHashCode(); }
                while (channels.ContainsKey(index));
            }

            GameObject go = null;
            AudioSource audioSource = null;

            if (channels.TryGetValue(index, out AudioInfo audioInfo))
            {
                go = audioInfo.go;
                audioSource = go.GetComponent<AudioSource>();

                audioSource.Stop();
                audioSource.clip = null;

                Core.StopCoroutine(audioInfo.coroutine);
                Core.onPhysicUpdate -= audioInfo.mainUpdate;
                audioInfo.coroutine = null;
                audioInfo.mainUpdate = null;
                audioInfo.onUpdate = null;
                audioInfo.onComplete = null;
            }
            else
            {
                go = Acquire();
                audioSource = go.GetComponent<AudioSource>();
                channels.Add(index, audioInfo = new AudioInfo(index, go));
            }

            go.transform.position = ownerTrans?.transform.position??pos??Vector3.zero;

            audioInfo.ownerTrans = null;
            audioInfo.mainUpdate = () => 
            { 
                if(ownerTrans != null)
                    go.transform.position = ownerTrans.position; 
                audioInfo.onUpdate?.Invoke(); 
            };

            Core.onPhysicUpdate += audioInfo.mainUpdate;

            audioSource.clip = audioClip;
            audioSource.spatialBlend = (ownerTrans == null && pos == null) ? 0.0f : 1.0f;
            audioSource.volume = 1.0f;
            audioSource.Play();

            if (loop)
            {
                Action loopCallback = null;
                audioInfo.coroutine = Core.StartCoroutine(audioSource.clip.length, loopCallback = () =>
                {
                    audioSource.Play();
                    audioInfo.coroutine = Core.StartCoroutine(audioSource.clip.length, loopCallback);
                    audioInfo.onComplete?.Invoke();
                });
            }
            else
            {
                audioInfo.coroutine = Core.StartCoroutine(audioSource.clip.length, () =>
                {
                    Recycle(audioInfo.go);
                    Core.onPhysicUpdate -= audioInfo.mainUpdate;
                    channels.Remove(audioInfo.index);
                    audioInfo.onComplete?.Invoke();
                });
            }

            audioInfo.stop = () =>
            {
                audioSource.Stop();
                audioSource.clip = null;
                Core.StopCoroutine(audioInfo.coroutine);
                Core.onPhysicUpdate -= audioInfo.mainUpdate;
                audioInfo.coroutine = null;
                audioInfo.mainUpdate = null;
                audioInfo.onUpdate = null;
                audioInfo.onComplete = null;
                Recycle(audioInfo.go);
                channels.Remove(audioInfo.index);
            };

            return new AudioChannel(audioInfo);
        }

        private GameObject Acquire()
        {
            GameObject go = null;
            while (validPool.Count > 0 && go == null)
                go = validPool.Dequeue();

            if (go != null)
            {
                go.SetActive(true);
                return go;
            }

            go = new GameObject("AudioEntity");
            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioPool.Add(go);
            go.transform.SetParent(audioPoolGo.transform);

            return go;
        }

        private void Recycle(GameObject go)
        {
            if (!audioPool.Contains(go))
                return;

            AudioSource audioSource = go.GetComponent<AudioSource>();
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.clip = null;
            go.SetActive(false);
            validPool.Enqueue(go);
        }
    }
}