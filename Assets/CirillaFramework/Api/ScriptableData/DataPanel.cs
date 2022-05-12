
using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class DataPanel : ScriptableObject
    {
        public List<DataPanelKV> dataList = new List<DataPanelKV>();

        public bool TryGetValue<T>(string key, int index, out T value)
        {
            foreach (DataPanelKV kv in dataList)
            {
                if (kv.key != key)
                    continue;

                SerializableData[] serializableDatas = kv.dataList.ToArray();
                if (index < 0 || index >= serializableDatas.Length)
                {
                    value = default(T);
                    return false;
                }

                try
                {
                    value = (T)serializableDatas[index].GetValue();
                }
                catch
                {
                    CiriDebugger.LogError($"{this.name}->{key}:类型转换");
                    value = default(T);
                    return false;
                }
                return true;
            }

            value = default(T);
            return false;
        }

        public bool ContainKey(string key)
        {
            foreach(DataPanelKV kv in dataList)
            {
                if (kv.key != key)
                    continue;

                return true;
            }

            return false;
        }
    }
}
