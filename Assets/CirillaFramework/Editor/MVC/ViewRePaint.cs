
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
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
            ViewEntity viewEntity = target as ViewEntity;

            string prefabPath = AssetDatabase.GetAssetPath(viewEntity.gameObject);
            if (prefabPath == null)
                prefabPath = PrefabStageUtility.GetPrefabStage(viewEntity.gameObject)?.assetPath;


            if (prefabPath == string.Empty)
            {
                EditorGUILayout.HelpBox("请制作成预制体并在预制体中进行资源收集", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginScrollView(scrollPos);
            GameObject resultObjct = null;
            EditorGUILayout.HelpBox("资源收集(拖取到以下框内进行添加)", MessageType.Info);
            EditorGUILayout.BeginVertical("FrameBox");
            EditorGUILayout.BeginFadeGroup(0.45f);
            resultObjct = (GameObject)EditorGUILayout.ObjectField(resultObjct, typeof(GameObject), GUILayout.Height(150));
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndVertical();



            //string dasd = PrefabStageUtility.GetPrefabStage(viewEntity.gameObject).assetPath;
            //string  = AssetDatabase.GetAssetPath(viewEntity.gameObject);

            if (resultObjct != null && resultObjct != viewEntity.gameObject && !viewEntity.ContainGo(resultObjct))
            {
                viewEntity.viewIndexInfos.Add(new ViewIndexInfo(FindKey(new List<string>(viewEntity.GetKeys()), resultObjct.name), resultObjct));
                EditorUtility.SetDirty(target);
            }

            if (viewEntity.viewIndexInfos.Count > 0)
            {
                EditorGUILayout.BeginVertical("HelpBox");
                for (int i = viewEntity.viewIndexInfos.Count - 1; i >= 0; i--)
                {
                    if (viewEntity.viewIndexInfos[i].go == null)
                    {
                        viewEntity.viewIndexInfos.RemoveAt(i);
                        EditorUtility.SetDirty(target);
                        continue;
                    }
                    EditorGUILayout.BeginHorizontal("HelpBox");
                    string text = EditorGUILayout.TextField(viewEntity.viewIndexInfos[i].key, GUILayout.Height(20), GUILayout.Width(120));
                    if (text != string.Empty && text != viewEntity.viewIndexInfos[i].key && !viewEntity.ContainKey(text))
                    {
                        viewEntity.viewIndexInfos[i].key = text;
                        EditorUtility.SetDirty(target);
                        break;
                    }
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(viewEntity.viewIndexInfos[i].go, typeof(Object));
                    GUI.enabled = true;
                    if (GUILayout.Button("-", GUILayout.Height(20), GUILayout.Width(20)))
                    {
                        viewEntity.viewIndexInfos.Remove(viewEntity.viewIndexInfos[i]);
                        EditorUtility.SetDirty(target);
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            string assemblyName = EditorUtil.devPath.Substring("Assets/".Length);
            string path;
            if (EditorUtil.devPath == string.Empty || !Directory.Exists(path = Application.dataPath + "/" + assemblyName))
            {
                EditorGUILayout.HelpBox("缺失开发目录，无法生成代码", MessageType.Error);
                goto Over;
            }

            bool mainPathExist = false;
            string mainCodePath = path + $"/{EditorUtil.mVCFolder}/{EditorUtil.viewFolder}/{viewEntity.gameObject.name}/{viewEntity.gameObject.name}.cs";
            string resourceCodePath = path + $"/{EditorUtil.mVCFolder}/{EditorUtil.viewFolder}/{viewEntity.gameObject.name}/{viewEntity.gameObject.name}.resource.cs";

            if (!File.Exists(mainCodePath))
                goto NodeCode;

            GUI.enabled = false;
            EditorGUILayout.BeginVertical("HelpBox");
            string assetPath = "Assets" + mainCodePath.Split(new[] { "Assets" }, System.StringSplitOptions.None)[1];
            MonoScript mainCodeScript = EditorUtil.LoadAsset<MonoScript>(assetPath);
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.ObjectField(mainCodeScript, typeof(MonoScript), GUILayout.Width(120), GUILayout.Height(20));
            EditorGUILayout.TextField(assetPath, GUILayout.Height(20));
            EditorGUILayout.EndHorizontal();
            EditorUtil.UnLoadAsset(mainCodeScript);
            mainPathExist = true;

            if (!File.Exists(resourceCodePath))
            {
                EditorGUILayout.HelpBox("附件丢失，请更新代码", MessageType.Error);
                goto CodeOver;
            }

            assetPath = "Assets" + resourceCodePath.Split(new[] { "Assets" }, System.StringSplitOptions.None)[1];
            MonoScript resourceScript = EditorUtil.LoadAsset<MonoScript>(assetPath);
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.ObjectField(resourceScript, typeof(MonoScript), GUILayout.Width(120), GUILayout.Height(20));
            EditorGUILayout.TextField(assetPath, GUILayout.Height(20));
            EditorGUILayout.EndHorizontal();
            EditorUtil.UnLoadAsset(resourceScript);

            CodeOver:
            EditorGUILayout.EndVertical();
            GUI.enabled = true;

            NodeCode:

            if (GUILayout.Button(mainPathExist? "更新代码" : "生成代码", GUILayout.Height(40)))
            {
                if (!mainPathExist)
                    WriteMainCode(mainCodePath, viewEntity.gameObject.name);
                WriteResourceCode(resourceCodePath, viewEntity.gameObject.name);
            }

            Over:
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

        private static void WriteMainCode(string path, string className)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string code =
            "using Cirilla;" + "\n" + "\n" + 
            $"namespace {Path.GetFileName(EditorUtil.devPath)}" + "\n" +
            "{" + "\n" +
            $"    public partial class {className} : {typeof(IView).Name}" + "\n" +
            "    {" + "\n" +
            "        #region 初始化与释放" + "\n" +
            "        public void Init()" + "\n" +
            "        {" + "\n" +
            "               Load();" + "\n" +
            "        }" + "\n" +
            "        public void Dispose()" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "        #endregion" + "\n" +
            "    }" + "\n" +
            "}" + "\n";

            Util.Write(path, code);
            AssetDatabase.Refresh();
        }

        private static void WriteResourceCode(string path, string className)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string code =
            "using Cirilla;" + "\n" + "\n" +
            $"namespace {Path.GetFileName(EditorUtil.devPath)}" + "\n" +
            "{" + "\n" +
            $"    public partial class {className} : {typeof(IView).Name}" + "\n" +
            "    {" + "\n" +
            "        private void Load()" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "    }" + "\n" +
            "}" + "\n";

            Util.Write(path, code);
            AssetDatabase.Refresh();
        }
    }
}

