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
                CiriDebugger.LogWarning("请求了一个空预制体");
                return null;
            }

            string poolName = prefab.name + indexInfo;

            if (!pools.TryGetValue(poolName, out GoPoolData poolData))
            {
                CiriDebugger.LogWarning($"不存在该对象池:{poolName} 请预先加载");
                return null;
            }

            GameObject go = null;
            while (poolData.validPool.Count > 0 && go == null)
                go = poolData.validPool.Dequeue();

            if (go != null)
            {
                go.SetActive(true);
                return go;
            }

            if (poolData.pool.Count >= poolData.pool.Capacity)
                return null;

            go = GameObject.Instantiate(prefab);
            go.AddComponent<GoPoolEntity>().Init(OnGoDestroy);
            go.name = poolName;
            poolData.pool.Add(go);
            go.SetActive(true);

            return go;
        }

        public void Recycle(GameObject go)
        {
            if (go == null)
            {
                CiriDebugger.LogWarning("回收实体为空");
                return;
            }

            if (!pools.TryGetValue(go.name, out GoPoolData poolData))
            {
                CiriDebugger.LogWarning("没有适合该实体的对象池:" + go.name);
                return;
            }

            if (!poolData.pool.Contains(go))
            {
                CiriDebugger.LogWarning("该实体并非对象池创建:" + go.name);
                return;
            }

            go.SetActive(false);
            poolData.validPool.Enqueue(go);
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

            pools.Add(poolName, new GoPoolData(capacity));
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

            for (int i = poolData.pool.Count - 1; i >= 0; i--)
                GameObject.Destroy(poolData.pool[i]);

            pools.Remove(poolname);
        }
        public void Shrink()
        {
            foreach (GoPoolData goPoolData in pools.Values)
            {
                GameObject go;
                while ((go = goPoolData.validPool.Dequeue()) != null)
                    GameObject.Destroy(go);
            }
        }

        public void Clear()
        {
            foreach(GoPoolData poolData in pools.Values)
            {
                for (int i = poolData.pool.Count - 1; i >= 0; i--)
                    GameObject.Destroy(poolData.pool[i]);
            }
            pools.Clear();
        }

        private void OnGoDestroy(GameObject go)
        {
            if (!pools.TryGetValue(go.name, out GoPoolData poolData))
                return;

            if (!poolData.pool.Contains(go))
                return;
            
            poolData.pool.Remove(go);
        }
    }
}