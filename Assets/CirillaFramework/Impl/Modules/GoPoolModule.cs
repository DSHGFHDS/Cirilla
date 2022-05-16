using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class GoPoolModule :IGoPoolModule
    {
        private const string indexInfo = "_GOPool";
        private Dictionary<string, GoPoolData> pools;
        private Vector3 vanityPos = new Vector3(-9999,-9999,-9999);
        public GoPoolModule()
        {
            pools = new Dictionary<string, GoPoolData>();
        }

        public GameObject Acquire(GameObject prefab)
        {
            if (prefab == null)
            {
                CiriDebugger.LogWarning("请求了一个空预制体");
                return null;
            }

            string poolName = prefab.name + indexInfo;

            if (!pools.TryGetValue(poolName, out GoPoolData poolData))
            {
                CiriDebugger.LogWarning($"不存在该对象池:{poolName} 请预先加载");
                return null;
            }

            foreach (KeyValuePair<GameObject, bool> kv in new Dictionary<GameObject, bool>(poolData.pool))
            {
                if (kv.Value)
                    continue;

                poolData.pool[kv.Key] = true;
                kv.Key.SetActive(true);
                return kv.Key;
            }

            if (poolData.pool.Count >= poolData.capacity)
                return null;

            GameObject go = GameObject.Instantiate(prefab);
            go.AddComponent<GoPoolEntity>().Init(OnGoDestroy);
            go.transform.position = vanityPos;
            go.name = poolName;
            go.SetActive(true);
            poolData.pool.Add(go, true);

            return go;
        }

        public void Recycle(GameObject go)
        {
            if (go == null)
            {
                CiriDebugger.LogWarning("回收了个空实体");
                return;
            }

            if (!pools.TryGetValue(go.name, out GoPoolData poolData))
            {
                CiriDebugger.LogWarning("没有适合该实体的容器:" + go.name);
                return;
            }

            if (!poolData.pool.ContainsKey(go))
            {
                CiriDebugger.LogWarning("该实体并非容器创建:" + go.name);
                return;
            }

            poolData.pool[go] = false;
            go.SetActive(false);
            go.transform.position = vanityPos;
        }

        public void Load(GameObject prefab, int capacity)
        {
            if (capacity <= 0)
            {
                CiriDebugger.LogWarning("容量错误");
                return;
            }

            if (prefab == null)
            {
                CiriDebugger.LogWarning("载入预制体为空");
                return;
            }

            string poolName = prefab.name + indexInfo;

            if (pools.ContainsKey(poolName))
            {
                CiriDebugger.LogWarning("预制体以载入池中:" + poolName);
                return;
            }

            pools.Add(poolName, new GoPoolData(new Dictionary<GameObject, bool>(capacity), capacity));
        }

        public void Unload(GameObject prefab)
        {
            if (prefab == null)
            {
                CiriDebugger.LogWarning("卸载不存在的预制体");
                return;
            }

            string poolname = prefab.name + indexInfo;

            if (!pools.TryGetValue(poolname, out GoPoolData poolData))
            {
                CiriDebugger.LogWarning("不存在该对象池:" + poolname);
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

        private void OnGoDestroy(GameObject go)
        {
            if (!pools.TryGetValue(go.name, out GoPoolData poolData))
                return;

            if (!poolData.pool.ContainsKey(go))
                return;
            
            poolData.pool.Remove(go);
        }
    }
}