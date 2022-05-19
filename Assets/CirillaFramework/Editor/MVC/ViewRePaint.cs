
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cirilla.CEditor
{
    [CustomEditor(typeof(ViewEntity))]
    public class ViewRePaint : Editor
    {
        private Vector2 scrollPos;
#pragma warning disable 0618
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginScrollView(scrollPos);
            GameObject resultObjct = null;
            EditorGUILayout.HelpBox("索引收集(拖取到以下框内进行添加)", MessageType.Info);
            EditorGUILayout.BeginVertical("FrameBox");
            EditorGUILayout.BeginFadeGroup(0.45f);
            resultObjct = (GameObject)EditorGUILayout.ObjectField(resultObjct, typeof(GameObject), GUILayout.Height(150));
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndVertical();
            ViewEntity viewEntity = target as ViewEntity;

            if (resultObjct != null && resultObjct != viewEntity.gameObject && resultObjct.transform.IsChildOf(viewEntity.transform) && !viewEntity.ContainGo(resultObjct))
                viewEntity.viewIndexInfos.Add(new ViewIndexInfo(FindKey(new List<string>(viewEntity.GetKeys()), resultObjct.name), resultObjct));

            if (viewEntity.viewIndexInfos.Count > 0)
            {
                EditorGUILayout.BeginVertical("HelpBox");
                for (int i = viewEntity.viewIndexInfos.Count - 1; i >= 0; i--)
                {
                    if (viewEntity.viewIndexInfos[i].go == null)
                    {
                        viewEntity.viewIndexInfos.RemoveAt(i);
                        continue;
                    }
                    EditorGUILayout.BeginHorizontal("HelpBox");
                    string text = EditorGUILayout.TextField(viewEntity.viewIndexInfos[i].key, GUILayout.Height(20), GUILayout.Width(120));
                    if (text != string.Empty && text != viewEntity.viewIndexInfos[i].key && !viewEntity.ContainKey(text))
                    {
                        viewEntity.viewIndexInfos[i].key = text;
                        break;
                    }
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(viewEntity.viewIndexInfos[i].go, typeof(Object));
                    GUI.enabled = true;
                    if (GUILayout.Button("-", GUILayout.Height(20), GUILayout.Width(20)))
                    {
                        viewEntity.viewIndexInfos.Remove(viewEntity.viewIndexInfos[i]);
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }



            EditorGUILayout.EndScrollView();
        }
#pragma warning restore 0618

        private static string FindKey(List<string> keys, string key, string oriKey = "", int index = 0)
        {
            if (keys.Contains(key))
            {
                if(oriKey == string.Empty)
                    oriKey = key;
                return FindKey(keys, oriKey + $"({index + 1})", oriKey, index + 1);
            }
            return key;
        }
    }
}

