
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirilla
{

    [CustomEditor(typeof(DataPanel))]
    public class DataPanelRepaint : Editor
    {
        private Vector2 scrollPos;
        private const string strNewData = "新数据";
        private const string instanceInfoDivider = "645awd78678&V18912Jw5";

        public override void OnInspectorGUI()
        {
            if(target.name == "GlobalTable")
            {
                EditorGUILayout.LabelField("请在全局配置表中进行修改");
                return;
            }
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DataPanel scriptableObj = target as DataPanel;
            for (int i = 0; i < scriptableObj.dataList.Count; i++)
            {
                DataPanelKV kv = scriptableObj.dataList[i];

                //折叠
                EditorGUILayout.BeginHorizontal();
                kv.foldout = EditorGUILayout.BeginFoldoutHeaderGroup(kv.foldout, kv.init ? kv.key : strNewData);
                if (!kv.foldout)
                {
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                if (GUILayout.Button("删除", new[] { GUILayout.Height(20), GUILayout.Width(35) }))
                {
                    scriptableObj.dataList.Remove(kv);
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndHorizontal();
                    break;
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndHorizontal();

                //字段
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("字段:", new[] { GUILayout.Height(20), GUILayout.Width(50) });
                string resultKey = EditorGUILayout.TextField(kv.init ? kv.key : strNewData).Trim();
                if (!string.IsNullOrEmpty(resultKey) && resultKey != strNewData && !scriptableObj.ContainKey(resultKey))
                {
                    kv.key = resultKey;
                    kv.init = true;
                }
                EditorGUILayout.EndHorizontal();

                //数据
                for (int j = 0; j < kv.dataList.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("值:", new[] { GUILayout.Height(20), GUILayout.Width(50) });

                    kv.dataList[j].type = (DataType)EditorGUILayout.EnumPopup(kv.dataList[j].type, new[] { GUILayout.Height(20), GUILayout.Width(kv.dataList[j].type.ToString().Length * 10 + 10) });

                    object value = kv.dataList[j].GetValue();
                    switch (kv.dataList[j].type)
                    {
                        case DataType.Int:
                            kv.dataList[j].SetValue(EditorGUILayout.IntField((int)value));
                            break;
                        case DataType.Long:
                            kv.dataList[j].SetValue(EditorGUILayout.LongField((long)value));
                            break;
                        case DataType.Float:
                            kv.dataList[j].SetValue(EditorGUILayout.FloatField((float)value));
                            break;
                        case DataType.Double:
                            kv.dataList[j].SetValue(EditorGUILayout.DoubleField((double)value));
                            break;
                        case DataType.Bool:
                            kv.dataList[j].SetValue(EditorGUILayout.Toggle((bool)value));
                            break;
                        case DataType.String:
                            kv.dataList[j].SetValue(EditorGUILayout.TextField(value.ToString()));
                            break;
                        case DataType.Object:
#pragma warning disable 0618
                            Object obj = EditorGUILayout.ObjectField((Object)value, typeof(Object));
                            if (obj != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj)))
                                kv.dataList[j].SetValue(obj.name + SerializableData.instanceInfoDivider + obj.GetInstanceID());
#pragma warning restore 0618
                            break;
                        case DataType.Color:
                            kv.dataList[j].SetValue(EditorGUILayout.ColorField((Color)value));
                            break;
                        case DataType.Vector2:
                            kv.dataList[j].SetValue(EditorGUILayout.Vector2Field("", (Vector2)value));
                            break;
                        case DataType.Vector3:
                            kv.dataList[j].SetValue(EditorGUILayout.Vector3Field("", (Vector3)value));
                            break;
                    }

                    if (kv.dataList.Count > 1 && GUILayout.Button("-", new[] { GUILayout.Height(20), GUILayout.Width(20) }))
                    {
                        kv.dataList.RemoveAt(j);
                        EditorGUILayout.EndHorizontal();
                        break;
                    }

                    if (j == kv.dataList.Count - 1)
                    {
                        if (GUILayout.Button("+", new[] { GUILayout.Height(20), GUILayout.Width(20) }))
                            kv.dataList.Add(new SerializableData(kv.dataList[j].type));
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加"))
                scriptableObj.dataList.Add(new DataPanelKV());
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            EditorUtility.SetDirty(target);
        }
    }
}
