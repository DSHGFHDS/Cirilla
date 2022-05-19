﻿using System;
using System.Collections.Generic;
using UnityEngine;

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

        public bool TryGetGo(string key, out GameObject go)
        {
            foreach (ViewIndexInfo viewIndexInfo in viewIndexInfos)
            {
                if (viewIndexInfo.key != key)
                    continue;

                go = viewIndexInfo.go;

                return true;
            }
            go = null;
            return false;
        }

        public bool TryGetKey(GameObject go, out string key)
        {
            foreach (ViewIndexInfo viewIndexInfo in viewIndexInfos)
            {
                if (viewIndexInfo.go != go)
                    continue;

                key = viewIndexInfo.key;

                return true;
            }
            key = string.Empty;
            return false;
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
    }
}