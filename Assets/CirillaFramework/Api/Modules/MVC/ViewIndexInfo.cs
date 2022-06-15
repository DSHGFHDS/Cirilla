
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirilla
{
    [Serializable]
    public class ViewIndexInfo
    {
        public string key;
        public GameObject go;
        public Object pointObj;

        public ViewIndexInfo(string key, GameObject go, Object pointObj)
        {
            this.key = key;
            this.go = go;
            this.pointObj = pointObj;
        }
    }
}
