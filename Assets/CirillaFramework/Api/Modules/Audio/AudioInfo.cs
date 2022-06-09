using System;
using UnityEngine;
namespace Cirilla
{
    public class AudioInfo
    {
        public int index { get; private set; }
        public GameObject go { get; private set; }
        public Transform ownerTrans;
        public Coroutine coroutine;
        public Action mainUpdate;
        public Action onUpdate;
        public Action onComplete;
        public Action stop;

        public AudioInfo(int index, GameObject go)
        {
            this.index = index;
            this.go = go;
        }
    }
}
