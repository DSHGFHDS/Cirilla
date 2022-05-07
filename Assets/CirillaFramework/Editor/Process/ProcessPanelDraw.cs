using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Cirilla.CEditor
{
    public class ProcessPanelDraw : EditorWindow
    {
        private string processTypePath;

        private List<ProcessInfoAttribute> processInfos;
        private Type processType;

        [MenuItem("Cirilla/流程配置表")]
        private static void Open()
        { 
            GetWindow<ProcessPanelDraw>("流程配置表").Show();
        }

        private static string tipInfo = $"流程配置表(添加后会在项目ProcessType中进行同步)";
        private Vector2 scrollPos;

        private void Init()
        {
            processInfos = new List<ProcessInfoAttribute>();
            processTypePath = Application.dataPath + "/GameLogic";

            if (!Directory.Exists(processTypePath))
                Directory.CreateDirectory(processTypePath);

            processType = Util.GetTypeFromName("ProcessType", "GameLogic");

            if (processType == null)
            {
                processTypePath += "/ProcessType.cs";
                File.Create(processTypePath).Close();
                Write();
                AssetDatabase.Refresh();
                return;
            }

            processTypePath = Util.SearchFileByType(processTypePath, processType);

            LoadAttributes(processType);
        }

        private void OnDestroy() {
            processType = null;
            AssetDatabase.Refresh();
        }

        public void OnGUI()
        {
            if (processType == null)
                Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.HelpBox(tipInfo, MessageType.Info);
            bool entrance = true;
            for (int i = 0; i < processInfos.Count; i ++)
            {
                ProcessInfoAttribute processInfo = processInfos[i];
                EditorGUILayout.BeginHorizontal();
                bool foldout = EditorGUILayout.BeginFoldoutHeaderGroup(processInfo.foldout, processInfo.type == null ? "请添加流程" : processInfo.type.Name);
                if (foldout != processInfo.foldout)
                {
                    processInfo.foldout = foldout;
                    Write();
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                if (!processInfo.foldout)
                {
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                if (GUILayout.Button("删除", new[] { GUILayout.Height(20), GUILayout.Width(35) }))
                {
                    processInfos.RemoveAt(i);
                    Write();
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(entrance? "入口:" : "预载:", new[] { GUILayout.Height(20), GUILayout.Width(50) });
                entrance = false;

                TextAsset script = null;
                if (processInfo.type != null)
                {
                    script = new TextAsset();
                    script.name = processInfo.type.Name;
                }

#pragma warning disable 0618
                Object processHandleInput = EditorGUILayout.ObjectField(script, typeof(TextAsset));
#pragma warning restore 0618

                if (processHandleInput?.name != processInfo.type?.Name)
                {
                    if (processHandleInput == null)
                    {
                        processInfo.type = null;
                        Write();
                    }
                    else
                    {
                        bool Repeat = false;
                        for (int j = 0; j < processInfos.Count; j++)
                        {
                            if (processInfos[j].type?.Name != processHandleInput.name)
                                continue;

                            Repeat = true;
                            break;
                        }
                        
                        if (!Repeat)
                        {
                            Type inputType = Util.GetTypeFromName(processHandleInput.name);
                            if (typeof(AProcessBase).IsAssignableFrom(inputType))
                            {
                                processInfo.type = inputType;
                                Write();
                            }
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加"))
                processInfos.Add(new ProcessInfoAttribute(null, true));

            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        private void Write()
        {
            StreamWriter streamWriter = new StreamWriter(processTypePath);
            string Message = $"using Cirilla;" + "\n" + "\n" +
                $"namespace GameLogic" + "\n" +
                "{" + "\n" +
                $"   public enum ProcessType" + "\n" +
                "   {" + "\n";
            foreach (ProcessInfoAttribute processInfo in processInfos)
            {
                if (processInfo.type == null)
                    continue;

                Message += $"        [{typeof(ProcessInfoAttribute).Name}(" + (processInfo.type != null ? $"typeof({processInfo.type.Name})" : "null") + $", {(processInfo.foldout ? "true" : "false")})]" + "\n";
                Message += $"        {processInfo.type.Name}" + ",\n";
            }
            Message += 
                "   }" + "\n" + 
                "}" + "\n";

            streamWriter.Write(Message);
            streamWriter.Close();
            
        }

        private void LoadAttributes(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                ProcessInfoAttribute attribute = fieldInfo.GetCustomAttribute<ProcessInfoAttribute>();
                if (attribute == null)
                    continue;

                processInfos.Add(attribute);
            }
        }
    }
}
