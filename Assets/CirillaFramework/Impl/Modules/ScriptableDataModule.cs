using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class ScriptableDataModule : IScriptableData
    {
        private const string globalTableName = "GlobalTable";
        private Dictionary<string, Dictionary<string, SerializableData[]>> configStock;
        private IRes res;
        public ScriptableDataModule(){
            configStock = new Dictionary<string, Dictionary<string, SerializableData[]>>();
            res = IocContainer.instance.Resolve<IRes>();
            Load("Config\\" + globalTableName);
        }

        private SerializableData[] this[string globalKey] { get { return this[globalTableName, globalKey]; } }
        
        private SerializableData[] this[string configName, string key]
        {
            get
            {
                if (!configStock.TryGetValue(configName, out Dictionary<string, SerializableData[]> config))
                {
                    CiriDebugger.LogError("file doesn't exist:" + configName);
                    return null;
                }

                if (!config.TryGetValue(key, out SerializableData[] dynamicData) || dynamicData.Length <= 0)
                {
                    CiriDebugger.LogError("key doesn't exist:" + key);
                    return null;
                }
                return dynamicData;
            }
        }
        
        public T GetValue<T>(string globalKey, int index = 0)
        {
            return GetValue<T>(globalTableName, globalKey, index);
        }
        
        public T GetValue<T>(string configName, string key, int index = 0)
        {
            SerializableData[] seriData = this[configName, key];

            if (seriData == null)
                return default(T);

            if (index >= seriData.Length)
            {
                CiriDebugger.LogError("index is out of range");
                return default(T);
            }

            return (T)seriData[index].GetValue();
        }

        public T[] GetValues<T>(string globalKey)
        {
            return GetValues<T>(globalTableName, globalKey);
        }

        public T[] GetValues<T>(string configName, string key)
        {
            SerializableData[] seriData = this[configName, key];

            if (seriData == null)
                return null;

            T[] values = new T[seriData.Length];
            for (int i = 0; i < seriData.Length; i++)
            {
                T value = (T)seriData[i].GetValue();
                values[i] = value;
            }

            if(values.Length <= 0)
                return null;

            return values;
        }

        public void Load(string packageName, string configName)
        {
            if (configStock.ContainsKey(configName))
                return;

            res.LoadPackage(packageName);
            Load(res.LoadAsset<DataPanel>(packageName, configName));
            res.UnloadAsset(packageName, configName);
        }

        public void Load(string resourcePath)
        {
            if (configStock.ContainsKey(resourcePath))
                return;

            Load(res.LoadAsset<DataPanel>(resourcePath));
            res.UnloadAsset(resourcePath);
        }

        public void Load(DataPanel configAsset)
        {
            if (configStock.ContainsKey(configAsset.name))
                return;

            configStock.Add(configAsset.name, new Dictionary<string, SerializableData[]>());
            foreach (DataPanelKV kv in configAsset.dataList)
                configStock[configAsset.name].Add(kv.key, kv.dataList.ToArray());
        }

        public void Remove(string configNameOrPath)
        {
            if (!configStock.ContainsKey(configNameOrPath))
            {
                CiriDebugger.LogWarning(configNameOrPath + "does not exist");
                return;
            }

            configStock.Remove(configNameOrPath);
        }

        public void Clear()
        {
            configStock.Clear();
        }
    }
}
