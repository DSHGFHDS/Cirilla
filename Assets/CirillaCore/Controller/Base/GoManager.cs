using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public partial class GoManager : ASingletonBase<GoManager>
    {
        private const string indexInfo = "_GOPool";
        private Dictionary<string, PoolData> pools;
        private GoManager()
        {
            pools = new Dictionary<string, PoolData>();
        }

        public GameObject Acquire(GameObject prefab)
        {
            if (prefab == null)
            {
                CiriDebugger.LogWarning("Acquire a null prefab");
                return null;
            }

            if (!IsPrefab(prefab))
            {
                CiriDebugger.LogWarning("Acquire a instance");
                return null;
            }

            string poolName = prefab.name + indexInfo;

            if (!pools.TryGetValue(poolName, out PoolData poolData))
            {
                CiriDebugger.LogWarning("There is no pool:" + poolName);
                return null;
            }

            foreach (KeyValuePair<GameObject, bool> kv in new Dictionary<GameObject, bool>(poolData.pool))
            {
                if (kv.Key == null)
                {
                    CiriDebugger.LogError("Null GameObject apeared.Don't fking destroy GameObject in the pool");
                    poolData.pool.Remove(kv.Key);
                    continue;
                }

                if (kv.Value)
                    continue;

                poolData.pool[kv.Key] = true;
                kv.Key.SetActive(true);
                return kv.Key;
            }

            if (poolData.pool.Count >= poolData.capacity)
                return null;
             
            GameObject go = GameObject.Instantiate(prefab);
            GameObject.DontDestroyOnLoad(go);
            go.name = poolName;
            go.SetActive(true);
            poolData.pool.Add(go, true);

            return go;
        }

        public void Recycle(GameObject go)
        {
            if (go == null)
            {
                CiriDebugger.LogWarning("Recycle a null GameObjcet");
                return;
            }

            if (!pools.TryGetValue(go.name, out PoolData poolData))
            {
                CiriDebugger.LogWarning("There is no pool:" + go.name);
                return;
            }

            if (!poolData.pool.ContainsKey(go))
            {
                CiriDebugger.LogWarning("The GameObject doesn't belong to pool:" + go.name);
                return;
            }

            poolData.pool[go] = false;
            go.SetActive(false);
        }

        public void Load(GameObject prefab, int capacity)
        {
            if (prefab == null)
            {
                CiriDebugger.LogWarning("Load a null prefab");
                return;
            }

            if (!IsPrefab(prefab))
            {
                CiriDebugger.LogWarning("Load a instance");
                return;
            }

            string poolName = prefab.name + indexInfo;

            if (pools.ContainsKey(poolName))
            {
                CiriDebugger.LogWarning("Already contained:" + poolName);
                return;
            }

            pools.Add(poolName, new PoolData(new Dictionary<GameObject, bool>(capacity), capacity));
        }

        public void Unload(GameObject prefab)
        {
            if (prefab == null)
            {
                CiriDebugger.LogWarning("Unload a null prefab");
                return;
            }

            if (!IsPrefab(prefab))
            {
                CiriDebugger.LogWarning("Unload a instance");
                return;
            }

            string poolname = prefab.name + indexInfo;

            if (!pools.TryGetValue(poolname, out PoolData poolData))
            {
                CiriDebugger.LogWarning("There is no pool:" + poolname);
                return;
            }

            foreach (GameObject go in poolData.pool.Keys) {
                if (go == null)
                    continue;
                GameObject.Destroy(go);
            }
            
            poolData.pool.Clear();
            pools.Remove(poolname);
        }

        public void Clear()
        {
            foreach (PoolData poolData in pools.Values)
            {
                foreach (GameObject go in poolData.pool.Keys)
                    GameObject.Destroy(go);

                poolData.pool.Clear();
            }
            pools.Clear();
        }

        private bool IsPrefab(GameObject prefab)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(prefab)))
            {
                CiriDebugger.LogWarning("Acquire a instance");
                return false;
            }

            return true;
#else
    return true;
#endif
        }
    }
}