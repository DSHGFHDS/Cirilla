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
        private List<ProcessInfoAttribute> processBuffer;
        [MenuItem("Cirilla/流程配置表")]
        private static void Open()
        { 
            GetWindow<ProcessPanelDraw>("流程配置表").Show();
        }

        private Vector2 scrollPos;

        private void OnDestroy() {
            AssetDatabase.Refresh();
        }

        public void OnGUI()
        {
            string processTypePath;
            if (EditorUtil.devPath == String.Empty || !Directory.Exists(processTypePath = Application.dataPath + "/" + EditorUtil.devPath.Substring("Assets/".Length)))
            {
                EditorGUILayout.HelpBox("请在项目配置表中设置开发目录", MessageType.Warning);
                return;
            }

            string processFile = processTypePath += "/ProcessType.cs";
            if (!File.Exists(processFile))
            {
                Write(processFile, null);
                AssetDatabase.Refresh();
                return;
            }

            Type processType = Util.GetTypeFromName("ProcessType", Path.GetFileName(EditorUtil.devPath));
            if(processType == null)
            {
                EditorGUI.HelpBox(new Rect(0, 0, 400, 20), "加载中...", MessageType.None);
                return;
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.HelpBox("流程配置表(添加后会在项目ProcessType中进行同步)", MessageType.Info);
            bool entrance = true;

            if(processBuffer == null)
                processBuffer = LoadAttributes(processType);

            for (int i = 0; i < processBuffer.Count; i ++)
            {
                ProcessInfoAttribute processInfo = processBuffer[i];
                EditorGUILayout.BeginHorizontal();
                bool foldout = EditorGUILayout.BeginFoldoutHeaderGroup(processInfo.foldout, processInfo.type == null ? "请添加流程" : processInfo.type.Name);
                if (foldout != processInfo.foldout)
                {
                    processInfo.foldout = foldout;
                    Write(processTypePath, processBuffer);
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
                    processBuffer.RemoveAt(i);
                    Write(processTypePath, processBuffer);
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
                        Write(processTypePath, processBuffer);
                    }
                    else
                    {
                        bool Repeat = false;
                        for (int j = 0; j < processBuffer.Count; j++)
                        {
                            if (processBuffer[j].type?.Name != processHandleInput.name)
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
                                Write(processTypePath, processBuffer);
                            }
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加"))
                processBuffer.Add(new ProcessInfoAttribute(null, true));

            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        public static void Write(string path, List<ProcessInfoAttribute> processInfos)
        {
            string Message = $"using Cirilla;" + "\n" + "\n" +
                $"namespace {Path.GetFileName(EditorUtil.devPath)}" + "\n" +
                "{" + "\n" +
                $"   public enum ProcessType" + "\n" +
                "   {" + "\n";
            if (processInfos != null)
            {
                foreach (ProcessInfoAttribute processInfo in processInfos)
                {
                    if (processInfo.type == null)
                        continue;

                    Message += $"        [{typeof(ProcessInfoAttribute).Name}(" + (processInfo.type != null ? $"typeof({processInfo.type.Name})" : "null") + $", {(processInfo.foldout ? "true" : "false")})]" + "\n";
                    Message += $"        {processInfo.type.Name}" + ",\n";
                }
            }
            Message += 
                "   }" + "\n" + 
                "}" + "\n";
            Util.Write(path, Message);
        }

        private List<ProcessInfoAttribute> LoadAttributes(Type type)
        {
            List<ProcessInfoAttribute> processInfos = new List<ProcessInfoAttribute>();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                ProcessInfoAttribute attribute = fieldInfo.GetCustomAttribute<ProcessInfoAttribute>();
                if (attribute == null)
                    continue;

                processInfos.Add(attribute);
            }

            return processInfos;
        }
    }
}
