
using System;
using UnityEngine;

namespace Cirilla
{
    [Serializable]
    public class ViewIndexInfo
    {
        public string key;
        public GameObject go;

        public ViewIndexInfo(string key, GameObject go)
        {
            this.key = key;
            this.go = go;
        }
    }
}
