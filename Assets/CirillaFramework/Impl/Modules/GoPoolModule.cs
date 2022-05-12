using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class GoPoolModule :IGoPoolModule
    {
        private const string indexInfo = "_GOPool";
        private Dictionary<string, GoPoolData> pools;
        public GoPoolModule()
        {
            pools = new Dictionary<string, GoPoolData>();
        }

        public GameObject Acquire(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Acquire a null prefab");
                return null;
            }

            string poolName = prefab.name + indexInfo;

            if (!pools.TryGetValue(poolName, out GoPoolData poolData))
            {
                Debug.LogWarning("There is no pool:" + poolName);
                return null;
            }

            foreach (KeyValuePair<GameObject, bool> kv in new Dictionary<GameObject, bool>(poolData.pool))
            {
                if (kv.Key == null)
                {
                    Debug.LogError("Null GameObject apeared.Don't destroy GameObject in the pool and clear the pool when switch scene");
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
            go.name = poolName;
            go.SetActive(true);
            poolData.pool.Add(go, true);

            return go;
        }

        public void Recycle(GameObject go)
        {
            if (go == null)
            {
                Debug.LogWarning("Recycle a null GameObjcet");
                return;
            }

            if (!pools.TryGetValue(go.name, out GoPoolData poolData))
            {
                Debug.LogWarning("There is no pool:" + go.name);
                return;
            }

            if (!poolData.pool.ContainsKey(go))
            {
                Debug.LogWarning("The GameObject doesn't belong to pool:" + go.name);
                return;
            }

            poolData.pool[go] = false;
            go.SetActive(false);
        }

        public void Load(GameObject prefab, int capacity)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Load a null prefab");
                return;
            }

            string poolName = prefab.name + indexInfo;

            if (pools.ContainsKey(poolName))
            {
                Debug.LogWarning("Already contained:" + poolName);
                return;
            }

            pools.Add(poolName, new GoPoolData(new Dictionary<GameObject, bool>(capacity), capacity));
        }

        public void Unload(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Unload a null prefab");
                return;
            }

            string poolname = prefab.name + indexInfo;

            if (!pools.TryGetValue(poolname, out GoPoolData poolData))
            {
                Debug.LogWarning("There is no pool:" + poolname);
                return;
            }

            List<GameObject> goBuffer = new List<GameObject>(poolData.pool.Keys);
            for (int i = goBuffer.Count-1; i >= 0; i --)
            {
                if (goBuffer[i] == null)
                    continue;

                GameObject.Destroy(goBuffer[i]);
            }

            poolData.pool.Clear();
            pools.Remove(poolname);
        }

        public void Clear()
        {
            List<string> poolBuffer = new List<string>(pools.Keys);
            for(int i = poolBuffer.Count - 1; i >= 0; i --)
            {
                List<GameObject> goBuffer = new List<GameObject>(pools[poolBuffer[i]].pool.Keys);
                for(int j = goBuffer.Count-1; j >=0; j --)
                {
                    if (goBuffer[i] == null)
                        continue;

                    GameObject.Destroy(goBuffer[i]);
                }
                pools[poolBuffer[i]].pool.Clear();
            }
            pools.Clear();
        }
    }
}