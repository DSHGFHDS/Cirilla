using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirilla
{
    public class ViewEntity : MonoBehaviour
    {
        public List<ViewIndexInfo> viewIndexInfos = new List<ViewIndexInfo>();

        private Action<GameObject> callback;
        public void Init(Action<GameObject> callback) => this.callback = callback;
        private void OnDestroy() => callback?.Invoke(gameObject);

        public bool ContainGo(GameObject go)
        {
            foreach (ViewIndexInfo viewIndexInfo in viewIndexInfos)
            {
                if (viewIndexInfo.go != go)
                    continue;

                return true;
            }

            return false;
        }

        public bool ContainKey(string key)
        {
            foreach (ViewIndexInfo viewIndexInfo in viewIndexInfos)
            {
                if (viewIndexInfo.key != key)
                    continue;

                return true;
            }

            return false;
        }

        public Object GetPointObj(string key)
        {
            foreach (ViewIndexInfo viewIndexInfo in viewIndexInfos)
            {
                if (viewIndexInfo.key != key)
                    continue;

                return viewIndexInfo.pointObj;
            }
            return null;
        }

        public GameObject GetGo(string key)
        {
            foreach (ViewIndexInfo viewIndexInfo in viewIndexInfos)
            {
                if (viewIndexInfo.key != key)
                    continue;

                return viewIndexInfo.go;
            }
            return null;
        }

        public string GetKey(GameObject go)
        {
            foreach (ViewIndexInfo viewIndexInfo in viewIndexInfos)
            {
                if (viewIndexInfo.go != go)
                    continue;

                return viewIndexInfo.key;
            }

            return null;
        }

        public string[] GetKeys()
        {
            string[] keys = new string[viewIndexInfos.Count];
            for (int i = 0; i < viewIndexInfos.Count; i++)
                keys[i] = viewIndexInfos[i].key;

            return keys;
        }
        
        public GameObject[] GetGos()
        {
            GameObject[] gos = new GameObject[viewIndexInfos.Count];
            for (int i = 0; i < viewIndexInfos.Count; i++)
                gos[i] = viewIndexInfos[i].go;

            return gos;
        }

        public Object[] GetPointObjs()
        {
            Object[] objs = new Object[viewIndexInfos.Count];
            for (int i = 0; i < viewIndexInfos.Count; i++)
                objs[i] = viewIndexInfos[i].pointObj;

            return objs;
        }
    }
}
