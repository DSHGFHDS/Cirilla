using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Cirilla
{
    public class CSVModule : ICSV
    {
        private IRes res;
        private Dictionary<Type, KeyValuePair<KeyValuePair<string, string[]>, Dictionary<object, object>>> csvDataStock;
        public CSVModule() {
            csvDataStock = new Dictionary<Type, KeyValuePair<KeyValuePair<string, string[]>, Dictionary<object, object>>>();
            res = IocContainer.instance.Resolve<IRes>();
        }
        public object[] GetValue<T>() where T : class
        {
            Type type = typeof(T);
            if (!csvDataStock.TryGetValue(type, out KeyValuePair<KeyValuePair<string, string[]>, Dictionary<object, object>> kv))
            {
                Debug.LogError(type + " does not exist");
                return null;
            }

            return new List<object>(kv.Value.Values).ToArray();
        }

        public T GetValue<T>(object primaryKey) where T : class
        {
            Type type = typeof(T);
            if (!csvDataStock.TryGetValue(type, out KeyValuePair<KeyValuePair<string, string[]>, Dictionary<object, object>> kv))
            {
                CiriDebugger.LogError(type + " does not exist");
                return null;
            }

            if (!kv.Value.TryGetValue(primaryKey, out object csvData))
                return null;

            return (T)csvData;
        }

        public void SetValue(object csvData)
        {
            Type type = csvData.GetType();
            if (!csvDataStock.TryGetValue(type, out KeyValuePair<KeyValuePair<string, string[]>, Dictionary<object, object>> kv))
            {
                CiriDebugger.LogError(type + " does not exist");
                return;
            }

            if(kv.Key.Key == null)
            {
                CiriDebugger.LogError("ABPackage can't be rewrited");
                return;
            }

            PropertyInfo[] props = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (props.Length < 1)
                return;

            object primaryKey = props[0].GetValue(csvData);
            if (!kv.Value.ContainsKey(primaryKey))
            {
                CiriDebugger.LogError("primaryKey:" + primaryKey + " does not exist");
                return;
            }

            csvDataStock[type].Value[primaryKey] = csvData;
            WriteCSV(type);
        }

        public void LoadCSV<T>(string packageName, string assetName) where T : class
        {
            if (csvDataStock.ContainsKey(typeof(T)))
                return;
            /*
            res.LoadPackage(packageName);
            TextAsset textAsset = res.LoadAsset<TextAsset>(packageName, assetName);
            string[] lines = textAsset.text.Split('\n');
            LoadStock<T>(null, lines);
            res.UnloadAsset(packageName, assetName);*/
        }

        public async void LoadCSV<T>(string filePath) where T : class
        {
            if (csvDataStock.ContainsKey(typeof(T)))
                return;

            filePath = Application.streamingAssetsPath + "/" + filePath;

            if (filePath == null || !File.Exists(filePath))
            {
                CiriDebugger.LogError(filePath + " does not exist");
                return;
            }
            
            StreamReader streamReader = new StreamReader(filePath, System.Text.Encoding.UTF8);
            List<Task<string>> taskPool = new List<Task<string>>();
            
            while (!streamReader.EndOfStream)
                taskPool.Add(streamReader.ReadLineAsync());

            string[] lines = new string[taskPool.Count];
            for (int i = 0; i < taskPool.Count; i++)
                lines[i] = await taskPool[i];

            LoadStock<T>(filePath, lines);

            streamReader.Close();
        }

        private void LoadStock<T>(string filePath, string[] lines) where T : class
        {
            if (lines.Length <= 2)
                return;

            Type type = typeof(T);
            string[] keys = lines[1].Split(',');
            PropertyInfo[] props = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (keys.Length != props.Length)
            {
                CiriDebugger.LogError(type.Name + " does not fixed");
                return;
            }

            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].Name == keys[i])
                    continue;

                CiriDebugger.LogError(type.Name + " does not fixed");
                return;
            }

            csvDataStock.Add(type, new KeyValuePair<KeyValuePair<string, string[]>, Dictionary<object, object>>(new KeyValuePair<string, string[]>(filePath, new string[2] { lines[0], lines[1] }), new Dictionary<object, object>()));

            for (int i = 2; i < lines.Length; i++)
            {
                if(string.IsNullOrEmpty(lines[i].Trim()))
                    continue;

                string[] values = lines[i].Split(',');

                if (values.Length != props.Length)
                {
                    CiriDebugger.LogError(filePath+"|Line:" + (i+1) + " error");
                    return;
                }

                T obj = Activator.CreateInstance<T>();
                for (int j = 0; j < props.Length; j++)
                {
                    string value = values[j].Trim();
                    if (props[j].PropertyType == typeof(bool))
                    {
                        switch (value)
                        {
                            case "1":
                                props[j].SetValue(obj, true, null);
                                break;
                            default:
                                props[j].SetValue(obj, false, null);
                                break;
                        }
                        continue;
                    }

                    if (props[j].PropertyType != typeof(string) && !int.TryParse(value, out int result))
                    {
                        props[j].SetValue(obj, Convert.ChangeType(0, props[j].PropertyType), null);
                        continue;
                    }

                    props[j].SetValue(obj, Convert.ChangeType(value, props[j].PropertyType), null);
                }
                csvDataStock[type].Value.Add(props[0].GetValue(obj), obj);
            }
        }

        private void WriteCSV(Type type)
        {
            if (!csvDataStock.TryGetValue(type, out KeyValuePair<KeyValuePair<string, string[]>, Dictionary<object, object>> kv))
                return;

            if (kv.Key.Key == null)
                return;

            List<Task> taskPool = new List<Task>();
            PropertyInfo[] props = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            StreamWriter streamWriter = new StreamWriter(kv.Key.Key, false, System.Text.Encoding.UTF8);
            taskPool.Add(streamWriter.WriteLineAsync(kv.Key.Value[0]));
            taskPool.Add(streamWriter.WriteLineAsync(kv.Key.Value[1]));
            foreach (object csvData in kv.Value.Values)
            {
                string line = string.Empty;
                for (int i = 0; i < props.Length; i++)
                {
                    string strData = string.Empty;
                    object data = props[i].GetValue(csvData);
                    if (data != null)
                    {
                        if(data.GetType() == typeof(bool))
                        {
                            switch((bool)data)
                            {
                                case true:
                                    strData = "1";
                                    break;
                                    case false:
                                    strData = "0";
                                    break;
                            }
                        }
                        else strData = data.ToString();
                    }

                    line += strData + (i == props.Length - 1 ? string.Empty : ",");
                }
                taskPool.Add(streamWriter.WriteLineAsync(line));
            }
            
            Task.WaitAll(taskPool.ToArray());
            streamWriter.Close();
        }

        public void Remove<T>()
        {
            Type type = typeof(T);
            if (!csvDataStock.ContainsKey(type))
                return;

            csvDataStock.Remove(type);
        }


        public void Clear()
        {
            csvDataStock.Clear();
        }
    }
}
