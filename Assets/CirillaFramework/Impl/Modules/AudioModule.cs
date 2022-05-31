
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class AudioModule : IAudioModule
    {
        private List<GameObject> audioPool;
        private Queue<GameObject> validPool;

        public AudioModule()
        {
            audioPool = new List<GameObject>();
            validPool = new Queue<GameObject>();
        }

        public void Play(GameObject owner, AudioClip audioClip, Action callback = null)
        {
            if (owner == null)
                return;

            GameObject go = Acquire();
            AudioSource audioSource = go.GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.Play();
            Core.StartCoroutine(audioClip.length, () =>
            {
                Recycle(go);
                callback?.Invoke();
            });
        }


        public void Play(Vector3 pos, AudioClip audioClip, Action callback = null)
        {
            GameObject go = Acquire();
            AudioSource audioSource = go.GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.Play();
            Core.StartCoroutine(audioClip.length, () =>
            {
                Recycle(go);
                callback?.Invoke();
            });
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