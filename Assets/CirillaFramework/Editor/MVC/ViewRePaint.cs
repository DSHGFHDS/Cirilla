
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
        private const string bindedPathName = "bindedPath";
        private const string resModuleName = "resModule";
        private const string codeViewPrefabName = "viewPrefab";
        private const string codeViewGameObjectName = "viewGameObjcet";
        private const string viewEntityName = "viewEntity";

#pragma warning disable 0618
        public override void OnInspectorGUI()
        {
            ViewEntity viewEntity = (ViewEntity)target;
            GameObject prefabObject = viewEntity.gameObject;
            string prefabPath = PrefabStageUtility.GetPrefabStage(prefabObject)?.assetPath??AssetDatabase.GetAssetPath(prefabObject);

            if (prefabPath == string.Empty)
            {
                EditorGUILayout.HelpBox("请在预制体中进行资源收集", MessageType.Warning);
                return;
            }

            if(!prefabPath.StartsWith($"{EditorUtil.devPath}/{EditorUtil.rawResourceFolder}/"))
            {
                EditorGUILayout.HelpBox($"预制体只有在资源文件夹:{EditorUtil.rawResourceFolder} 中才能被正确读取", MessageType.Warning);
                return;
            }

            GameObject resultObjct = null;
            EditorGUILayout.HelpBox("资源收集(拖取到以下框内进行添加)", MessageType.Info);
            EditorGUILayout.BeginVertical("FrameBox");
            EditorGUILayout.BeginFadeGroup(0.45f);
            resultObjct = (GameObject)EditorGUILayout.ObjectField(resultObjct, typeof(GameObject), GUILayout.Height(150));
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndVertical();

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
                    if (text != string.Empty && text != viewEntity.viewIndexInfos[i].key && !viewEntity.ContainKey(text) && Util.IsMatchStatementRule(text)
                        && text != bindedPathName && text != resModuleName && text != codeViewPrefabName && text != codeViewGameObjectName && text != viewEntityName)
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
                return;
            }

            bool codeExist = false;
            string resourceCodePath = path + $"/{EditorUtil.mVCFolder}/{EditorUtil.viewFolder}/{prefabObject.name}/{prefabObject.name}.resource.cs";
            string mainCodePath = path + $"/{EditorUtil.mVCFolder}/{EditorUtil.viewFolder}/{prefabObject.name}/{prefabObject.name}.cs";
            string resPath = prefabPath.Split(new[] { EditorUtil.rawResourceFolder + "/" }, System.StringSplitOptions.None)[1];

            if (!File.Exists(resourceCodePath) || !File.Exists(mainCodePath))
                goto NodeCode;

            string assetPath = "Assets" + resourceCodePath.Split(new[] { "Assets" }, System.StringSplitOptions.None)[1];
            MonoScript monoScript = EditorUtil.LoadAsset<MonoScript>(assetPath);
            if (!monoScript.text.Replace(" ", "").Contains($"bindedPath=\"{resPath}\""))
            {
                EditorUtil.UnLoadAsset(monoScript);
                EditorGUILayout.HelpBox("预制体重名冲突，无法生成代码:\n" + assetPath, MessageType.Error);
                return;
            }

            codeExist = true;
            EditorGUILayout.BeginVertical("HelpBox");
            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.ObjectField(monoScript, typeof(MonoScript), GUILayout.Width(120), GUILayout.Height(20));
            EditorGUILayout.TextField(assetPath, GUILayout.Height(20));
            EditorGUILayout.EndHorizontal();
            EditorUtil.UnLoadAsset(monoScript);

            assetPath = "Assets" + mainCodePath.Split(new[] { "Assets" }, System.StringSplitOptions.None)[1];
            monoScript = EditorUtil.LoadAsset<MonoScript>(assetPath);
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.ObjectField(monoScript, typeof(MonoScript), GUILayout.Width(120), GUILayout.Height(20));
            EditorGUILayout.TextField(assetPath, GUILayout.Height(20));
            EditorGUILayout.EndHorizontal();
            EditorUtil.UnLoadAsset(monoScript);
            GUI.enabled = true;
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("清理代码", "flow node hex 6", GUILayout.Width(EditorGUIUtility.currentViewWidth-40), GUILayout.Height(40)))
            {
                string deleteDir = Path.GetDirectoryName(mainCodePath);
                Directory.Delete(deleteDir, true);
                File.Delete(deleteDir + ".meta");
                AssetDatabase.Refresh();
            }

            NodeCode:
            if (GUILayout.Button(codeExist? "更新代码" : "生成代码", codeExist ? "flow node hex 1": "flow node hex 3", GUILayout.Width(EditorGUIUtility.currentViewWidth - 40), GUILayout.Height(40)))
            {
                if (!codeExist)
                    WriteMainCode(mainCodePath, prefabObject.name);
                WriteResourceCode(resourceCodePath, resPath, viewEntity, prefabObject.name);
            }
        }

#pragma warning restore 0618

        private static string FindKey(List<string> keys, string key, string oriKey = "", int index = 0)
        {
            if (!Util.IsMatchStatementRule(key))
                key = "key";
            else key = key.ToLower()[0] + key.Substring(1);

            if (keys.Contains(key))
            {
                if(oriKey == string.Empty)
                    oriKey = key;
                return FindKey(keys, oriKey + $"_{index + 1}", oriKey, index + 1);
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
            "        public void VeiwInit()" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "        public void VeiwDispose()" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "        #endregion" + "\n" +
            "    }" + "\n" +
            "}" + "\n";

            EditorUtil.Write(path, code);
            AssetDatabase.Refresh();
        }

        private static void WriteResourceCode(string path, string resPath, ViewEntity viewEntity, string className)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string gameObjectName = typeof(GameObject).Name;
            string code =
            "using Cirilla;" + "\n" +
            "using UnityEngine;" + "\n" + "\n" +
            $"namespace {Path.GetFileName(EditorUtil.devPath)}" + "\n" +
            "{" + "\n" +
            $"    public partial class {className} : {typeof(IView).Name}" + "\n" +
            "    {" + "\n" +
            $"        private const string {bindedPathName} = \"{resPath}\";" + "\n" + "\n" +
            $"        [{typeof(DependencyAttribute).Name}] {typeof(IResModule).Name} {resModuleName};" + "\n" + "\n" +
            $"        private {gameObjectName} {codeViewPrefabName};" + "\n" +
            $"        private {gameObjectName} {codeViewGameObjectName};" + "\n" + "\n";
            foreach (ViewIndexInfo viewIndexInfo in viewEntity.viewIndexInfos) 
                code += $"        private {gameObjectName} {viewIndexInfo.key};" + "\n";

            code +=
            "        public void Init()" + "\n" +
            "        {" + "\n" +
            $"            {codeViewPrefabName} = {resModuleName}.LoadAsset<{gameObjectName}>({bindedPathName});" + "\n" +
            $"            if({codeViewPrefabName} == null)" + "\n" +
            "            {" + "\n" +
            $"                CiriDebugger.LogError(\"{className}加载失效\");" + "\n" +
            "                return;" + "\n" +
            "            }" + "\n" +
            $"            {codeViewGameObjectName} = {typeof(Core).Name}.CirillaGiveBirth({codeViewPrefabName});" + "\n";
            if (viewEntity.viewIndexInfos.Count > 0)
                code += $"            {typeof(ViewEntity).Name} {viewEntityName} = {codeViewGameObjectName}.GetComponent<{typeof(ViewEntity).Name}>();" + "\n";

            foreach (ViewIndexInfo viewIndexInfo in viewEntity.viewIndexInfos)
                code += $"            {viewIndexInfo.key} = {viewEntityName}.GetGo(\"{viewIndexInfo.key}\");" + "\n";

            code +=
            "            VeiwInit();" + "\n" +
            "        }" + "\n" +
            "        public void Dispose()" + "\n" +
            "        {" + "\n" +
            $"            if({codeViewPrefabName} == null)" + "\n" +
            "                return;" + "\n" +
            $"            {gameObjectName}.Destroy({codeViewGameObjectName});" + "\n" +
            $"            {resModuleName}.UnLoadAsset({codeViewPrefabName});" + "\n" +
            "            VeiwDispose();" + "\n" +
            "        }" + "\n" +
            "    }" + "\n" +
            "}" + "\n";

            EditorUtil.Write(path, code);
            AssetDatabase.Refresh();
        }
    }
}

