using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;


namespace Cirilla
{
    public class GlobalDataPanel : EditorWindow
    {
        [MenuItem("Cirilla/ConfigPanel")]
        private static void Open(){
            EditorWindow.GetWindow<GlobalDataPanel>();
        }
        private const string strNewData = "新数据";
        private GlobalData globalData;

        private void Awake()
        {
            globalData = GameObject.Find(ASingletonEntity.gameEntname).GetComponent<GlobalData>();
        }

        public void OnGUI()
        {
            for (int i = globalData.kvBuffer.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                ConfigKV kv = globalData.kvBuffer[i];
                kv.foldout = EditorGUILayout.Foldout(kv.foldout, kv.foldout ? "" : kv.init ? kv.key : strNewData);

                if (!kv.foldout)
                {
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("删除"))
                {
                    globalData.kvBuffer.Remove(kv);
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("字段:", new[] { GUILayout.Height(20), GUILayout.Width(50) });
                string resultKey = EditorGUILayout.TextField(kv.init ? kv.key : strNewData).Trim();
                if (resultKey != "" && resultKey != kv.key && resultKey != strNewData)
                {
                    kv.key = resultKey;
                    kv.init = true;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("值:", new[] { GUILayout.Height(20), GUILayout.Width(50) });
                kv.type = (ConfigDataType)EditorGUILayout.EnumPopup(kv.type, new[] { GUILayout.Height(20), GUILayout.Width(kv.type.ToString().Length * 10 + 10) });
                switch (kv.type)
                {
                    case ConfigDataType.Int:
                        kv.SetValue(EditorGUILayout.IntField((int)kv.GetValue()));
                        break;
                    case ConfigDataType.Long:
                        kv.SetValue(EditorGUILayout.LongField((long)kv.GetValue()));
                        break;
                    case ConfigDataType.Float:
                        kv.SetValue(EditorGUILayout.FloatField((float)kv.GetValue()));
                        break;
                    case ConfigDataType.Double:
                        kv.SetValue(EditorGUILayout.DoubleField((double)kv.GetValue()));
                        break;
                    case ConfigDataType.Bool:
                        kv.SetValue(EditorGUILayout.Toggle((bool)kv.GetValue()));
                        break;
                    case ConfigDataType.String:
                        kv.SetValue(EditorGUILayout.TextField(kv.GetValue().ToString()));
                        break;
                    case ConfigDataType.Object:
#pragma warning disable 0618
                        kv.SetValue(EditorGUILayout.ObjectField((Object)kv.GetValue(), typeof(Object)));
#pragma warning restore 0618
                        break;
                    case ConfigDataType.Color:
                        kv.SetValue(EditorGUILayout.ColorField((Color)kv.GetValue()));
                        break;
                    case ConfigDataType.Vector2:
                        kv.SetValue(EditorGUILayout.Vector2Field("", (Vector2)kv.GetValue()));
                        break;
                    case ConfigDataType.Vector3:
                        kv.SetValue(EditorGUILayout.Vector3Field("", (Vector3)kv.GetValue()));
                        break;
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加"))
            {
                ConfigKV configKV = new ConfigKV();
                configKV.key = Guid.NewGuid().ToString();
                globalData.kvBuffer.Add(configKV);
            }
            GUILayout.EndHorizontal();
        }
    }
}
