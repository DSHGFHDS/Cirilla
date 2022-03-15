using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class ConfigManager : ASingletonBase<ConfigManager>
    {
        public const string congfigSetting = "Setting";
        private Dictionary<string, Dictionary<string, object>> configStock;
        private ConfigManager()
        {
            configStock = new Dictionary<string, Dictionary<string, object>>();
        }

        public object this[string configName, string key]
        {
            get
            {
                if (!configStock.TryGetValue(configName, out Dictionary<string, object> config))
                {
                    if(configName == congfigSetting)
                    {
                        LoadConfig(congfigSetting);
                        return configStock[congfigSetting][key];
                    }
                    return null;
                }

                if (!config.TryGetValue(key, out object obj))
                    return null;

                return obj;
            }
        }


        public void LoadConfig(ConfigData configAsset)
        {
            if (configStock.ContainsKey(configAsset.name))
                return;

            configStock.Add(configAsset.name, new Dictionary<string, object>());
            ConfigData config = ScriptableObject.Instantiate<ConfigData>(configAsset);
            foreach (ConfigKV kv in config.kvBuffer)
                configStock[configAsset.name].Add(kv.key, kv.GetValue());
        }

        public void LoadConfig(string packageName, string configName)
        {
            ResManager.instance.LoadPackage(packageName);
            ConfigData config = ResManager.instance.LoadAsset<ConfigData>(packageName, configName);
            if (config == null)
                return;

            LoadConfig(config);
        }

        public void LoadConfig(string resourcePath)
        {
            ConfigData config = ResManager.instance.LoadAsset<ConfigData>(resourcePath);
            if (config == null)
                return;

            LoadConfig(config);
        }
        
        public void UnloadConfig(string configName)
        {
            configStock.Remove(configName);
        }

        public void ClearConfig()
        {
            configStock.Clear();
        }
    }
}
