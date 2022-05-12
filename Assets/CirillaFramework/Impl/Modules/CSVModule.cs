using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Cirilla
{
    public class CSVModule : ICSVModule
    {
        public CSVModule() {
        }

        public T[] Load<T>(TextAsset textAsset) where T : class
        {
            if (textAsset == null)
                return null;

            string[] lines = textAsset.text.Split('\n');
            return ReadText<T>(lines);
        }

        public async Task<T[]> Load<T>(string path) where T : class
        {
            path = Application.streamingAssetsPath + "/" + path;

            if (path == null || !File.Exists(path))
            {
                CiriDebugger.LogError("文件不存在:" + path);
                return null;
            }
            
            StreamReader streamReader = new StreamReader(path, System.Text.Encoding.UTF8);
            List<Task<string>> taskPool = new List<Task<string>>();
            
            while (!streamReader.EndOfStream)
                taskPool.Add(streamReader.ReadLineAsync());

            string[] lines = new string[taskPool.Count];
            for (int i = 0; i < taskPool.Count; i++)
                lines[i] = await taskPool[i];
            streamReader.Close();

            return ReadText<T>(lines);
        }

        private T[] ReadText<T>(string[] lines) where T : class
        {
            if (lines.Length <= 2)
                return null;

            Type type = typeof(T);
            string[] keys = lines[1].Split(',');
            PropertyInfo[] props = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (keys.Length != props.Length)
            {
                CiriDebugger.LogError("类型不匹配:" + type.Name);
                return null;
            }

            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].Name == keys[i])
                    continue;

                CiriDebugger.LogError("类型不匹配:" + type.Name);
                return null;
            }

            List<T> objs = new List<T>();

            for (int i = 2; i < lines.Length; i++)
            {
                if(string.IsNullOrEmpty(lines[i].Trim()))
                    continue;

                string[] values = lines[i].Split(',');

                if (values.Length != props.Length)
                {
                    CiriDebugger.LogError($"读取{type.Name}时，出现错误:{(i + 1)}行");
                    return null;
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

                objs.Add(obj);
            }

            return objs.ToArray();
        }

        public void Write<T>(T[] csvDatas, string path)
        {
            path = Application.streamingAssetsPath + "/" + path;

            Type type = csvDatas.GetType();
            PropertyInfo[] props = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (props.Length < 1)
                return;

            StreamWriter streamWriter;
            try
            {
                streamWriter = new StreamWriter(path, false, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                CiriDebugger.LogError(ex);
                return;
            }

            List<Task> taskPool = new List<Task>();
            string message = string.Empty;
            for(int i = 0; i < props.Length; i ++)
            {
                message += props[i].Name;
                if (i == props.Length-1)
                    break;
                message += ",";
            }
            taskPool.Add(streamWriter.WriteLineAsync(message));

            foreach (T csvData in csvDatas)
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
    }
}
