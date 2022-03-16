using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class ConfigManager : ASingletonBase<ConfigManager>
    {
        private Dictionary<string, Dictionary<string, object>> configStock;
        private Dictionary<string, object> globalData;
        private ConfigManager(){
            configStock = new Dictionary<string, Dictionary<string, object>>();
        }
        
        public object this[string key]
        {
            get
            {
                if (globalData == null)
                {
                    globalData = new Dictionary<string, object>();
                    foreach (ConfigKV kv in ASingletonEntity.goInstance.GetComponent<GlobalData>().kvBuffer)
                        globalData.Add(kv.key, kv.GetValue());
                }

                if (!globalData.TryGetValue(key, out object obj))
                    return null;

                return obj;
            }
        }

        public object this[string configName, string key]
        {
            get
            {
                if (!configStock.TryGetValue(configName, out Dictionary<string, object> config))
                    return null;

                if (!config.TryGetValue(key, out object obj))
                    return null;

                return obj;
            }
        }

        public void Load(ConfigData configAsset)
        {
            if (configStock.ContainsKey(configAsset.name))
                return;

            configStock.Add(configAsset.name, new Dictionary<string, object>());
            ConfigData config = ScriptableObject.Instantiate<ConfigData>(configAsset);
            foreach (ConfigKV kv in config.kvBuffer)
                configStock[configAsset.name].Add(kv.key, kv.GetValue());
        }

        public void Load(string packageName, string configName)
        {
            ResManager.instance.LoadPackage(packageName);
            ConfigData config = ResManager.instance.LoadAsset<ConfigData>(packageName, configName);
            if (config == null)
                return;

            Load(config);
        }

        public void Load(string resourcePath)
        {
            ConfigData config = ResManager.instance.LoadAsset<ConfigData>(resourcePath);
            if (config == null)
                return;

            Load(config);
        }
        
        public void Unload(string configName)
        {
            configStock.Remove(configName);
        }

        public void Clear()
        {
            configStock.Clear();
        }
    }
}
