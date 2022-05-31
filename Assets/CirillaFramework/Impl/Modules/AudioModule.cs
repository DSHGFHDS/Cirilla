
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioModule : IAudioModule
{
    private List<GameObject> audioPool;
    private Queue<GameObject> validPool;

    public AudioModule()
    {
        audioPool = new List<GameObject>();
        validPool = new Queue<GameObject>();
    }

    public void Play(GameObject gameObject, AudioClip audioClip, Action callback = null)
    {
        
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
