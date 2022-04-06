
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
                    continue;
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

                    DataType lastType = kv.dataList[j].type;
                    DataType changedType = (DataType)EditorGUILayout.EnumPopup(lastType, new[] { GUILayout.Height(20), GUILayout.Width(lastType.ToString().Length * 10 + 10) });

                    object value = kv.dataList[j].GetValue();
                    switch (changedType)
                    {
                        case DataType.Int:
                            kv.dataList[j].SetValue(EditorGUILayout.IntField(lastType == changedType ? (int)value : 0));
                            break;
                        case DataType.Long:
                            kv.dataList[j].SetValue(EditorGUILayout.LongField(lastType == changedType ? (long)value : 0L));
                            break;
                        case DataType.Float:
                            kv.dataList[j].SetValue(EditorGUILayout.FloatField(lastType == changedType ? (float)value : 0f));
                            break;
                        case DataType.Double:
                            kv.dataList[j].SetValue(EditorGUILayout.DoubleField(lastType == changedType ? (double)value : 0d));
                            break;
                        case DataType.Bool:
                            kv.dataList[j].SetValue(EditorGUILayout.Toggle(lastType == changedType ? (bool)value : false));
                            break;
                        case DataType.String:
                            kv.dataList[j].SetValue(EditorGUILayout.TextField(lastType == changedType ? value.ToString() : ""));
                            break;
                        case DataType.Object:
#pragma warning disable 0618
                            Object obj = EditorGUILayout.ObjectField(lastType == changedType ? (Object)value : Util.unNullUnityObject, typeof(Object), false);
                            kv.dataList[j].SetValue(obj == null ? Util.unNullUnityObject : obj);
#pragma warning restore 0618
                            break;
                        case DataType.Color:
                            kv.dataList[j].SetValue(EditorGUILayout.ColorField(lastType == changedType ? (Color)value : Color.white));
                            break;
                        case DataType.Vector2:
                            kv.dataList[j].SetValue(EditorGUILayout.Vector2Field("", lastType == changedType ? (Vector2)value : Vector2.zero));
                            break;
                        case DataType.Vector3:
                            kv.dataList[j].SetValue(EditorGUILayout.Vector3Field("", lastType == changedType ? (Vector3)value : Vector3.zero));
                            break;
                    }

                    if (j == kv.dataList.Count - 1)
                    {
                        if (GUILayout.Button("+", new[] { GUILayout.Height(20), GUILayout.Width(20) }))
                            kv.dataList.Add(new SerializableData(changedType));

                        EditorGUILayout.EndHorizontal();
                        continue;
                    }

                    if (GUILayout.Button("-", new[] { GUILayout.Height(20), GUILayout.Width(20) }))
                        kv.dataList.RemoveAt(j);

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
