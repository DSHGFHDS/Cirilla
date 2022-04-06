using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Cirilla
{
    public class NetPanelDraw : EditorWindow
    {
        private static string netTypePath;
        private static Type netType;
        private static List<NetInfoAttribute> netInfos;

        [MenuItem("Cirilla/网络配置表")]
        private static void Open(){
            GetWindow<NetPanelDraw>("网络配置表").Show();
        }

        private const string tipInfo = "网络配置表(添加网络后会在NetType中进行同步)";
        private Vector2 scrollPos;

        private void Init()
        {
            netType = Util.GetTypeFromName("NetType", "GameLogic");
            if (netType == null)
                netTypePath = (Application.dataPath + "\\" + "GameLogic\\" + MethodBase.GetCurrentMethod().DeclaringType.Namespace).Replace("/", "\\");
            else
                netTypePath = Util.SearchFileByType(netType);

            LoadAttributes(netType);
        }

        private void OnDestroy()
        {
            AssetDatabase.Refresh();
        }

        public void OnGUI()
        {
            if (netType == null)
                Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.HelpBox(tipInfo, MessageType.Info);

            for (int i = 0; i < netInfos.Count; i++)
            {
                NetInfoAttribute netInfo = netInfos[i];
                EditorGUILayout.BeginHorizontal();
                bool foldout = EditorGUILayout.BeginFoldoutHeaderGroup(netInfo.foldout, netInfo.type == null ? "请添加网络" : netInfo.type.Name);
                if (foldout != netInfo.foldout)
                {
                    netInfo.foldout = foldout;
                    Write();
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                if (!netInfo.foldout)
                {
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                if (GUILayout.Button("删除", new[] { GUILayout.Height(20), GUILayout.Width(35) }))
                {
                    netInfos.RemoveAt(i);
                    Write();
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("地址:", new[] { GUILayout.Height(20), GUILayout.Width(50) });

                string urlInput = EditorGUILayout.TextField(netInfo.url);
                if(urlInput != netInfo.url)
                {
                    netInfo.url = urlInput;
                    Write();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("网络:", new[] { GUILayout.Height(20), GUILayout.Width(50) });

                TextAsset script = null;
                if (netInfo.type != null)
                {
                    script = new TextAsset();
                    script.name = netInfo.type.Name;
                }

#pragma warning disable 0618
                Object netHandleInput = EditorGUILayout.ObjectField(script, typeof(TextAsset));
#pragma warning restore 0618

                if (netHandleInput?.name != netInfo.type?.Name)
                {
                    if (netHandleInput == null)
                    {
                        netInfo.type = null;
                        Write();
                    }
                    else
                    {
                        bool Repeat = false;
                        for (int j = 0; j < netInfos.Count; j++)
                        {
                            if (netInfos[j].type?.Name != netHandleInput.name)
                                continue;

                            Repeat = true;
                            break;
                        }

                        if (!Repeat)
                        {
                            Type inputType = Util.GetTypeFromName(netHandleInput.name);
                            if (typeof(INetBase).IsAssignableFrom(inputType))
                            {
                                netInfo.type = inputType;
                                Write();
                            }
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加"))
                netInfos.Add(new NetInfoAttribute(null, "ws://127.0.0.1:8888", true));

            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        private void Write()
        {
            StreamWriter streamWriter = new StreamWriter(netTypePath);
            string Message = "\n" +
                $"namespace {MethodBase.GetCurrentMethod().DeclaringType.Namespace}" + "\n" +
                "{" + "\n" +
                $"   public enum {netType.Name}" + "\n" +
                "   {" + "\n";
            foreach (NetInfoAttribute netInfo in netInfos)
            {
                if (netInfo.type == null)
                    continue;

                Message += $"        [{typeof(NetInfoAttribute).Name}(" + (netInfo.type != null ? $"typeof({netInfo.type.Name})" : "null") + $", \"{netInfo.url}\", {(netInfo.foldout ? "true" : "false")})]" + "\n";
                Message += $"        {netInfo.type.Name}" + ",\n";
            }
            Message +=
                "   }" + "\n" +
                "}" + "\n";

            streamWriter.Write(Message);
            streamWriter.Close();

        }

        private void LoadAttributes(Type type)
        {
            if (type == null)
                return;

            netInfos = new List<NetInfoAttribute>();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                NetInfoAttribute attribute = fieldInfo.GetCustomAttribute<NetInfoAttribute>();
                if (attribute == null)
                    continue;

                netInfos.Add(attribute);
            }
        }
    }
}
