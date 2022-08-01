 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

namespace Cirilla.CEditor
{
    public class Packager : EditorWindow
    {
#if UNITY_ANDROID
        private static BuildTarget selectedBuildTarget = BuildTarget.Android;
#elif UNITY_STANDALONE_WIN
        private static BuildTarget selectedBuildTarget = BuildTarget.StandaloneWindows64;
#elif UNITY_IOS
        private static BuildTarget selectedBuildTarget = BuildTarget.iOS;
#elif UNITY_STANDALONE_OSX
        private static BuildTarget selectedBuildTarget = BuildTarget.StandaloneOSX;
#else
        private static BuildTarget selectedBuildTarget = BuildTarget.NoTarget;
#endif
        private static string pkLog;
        [MenuItem("Cirilla/工具/资源管理器", false, 61)]
        public static void Open() => GetWindow<Packager>("资源配置表").Show();

        public void OnGUI()
        {
            EditorGUILayout.HelpBox("编辑器模式下，资源可不通过打包进行快速加载测试", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("懒加载模式", new[] { GUILayout.Height(20), GUILayout.Width(70) });
            if (EditorUtil.lazyLoad != EditorGUILayout.Toggle(EditorUtil.lazyLoad, new[] { GUILayout.Height(20), GUILayout.Width(20) }))
            EditorUtil.lazyLoad = !EditorUtil.lazyLoad;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("选择平台进行资源打包(版本号生成文件后续可用于热更校验)", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("平台", new[] { GUILayout.Height(20), GUILayout.Width(40) });
            selectedBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(selectedBuildTarget);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            int versionValue = EditorGUILayout.IntField(EditorUtil.version);
            if (EditorUtil.version != versionValue) EditorUtil.version = versionValue;
            EditorGUILayout.EndHorizontal();

            if (selectedBuildTarget == BuildTarget.NoTarget)
                GUI.enabled = false;

            if (GUILayout.Button("打包"))
                Packgae(selectedBuildTarget);

            GUI.enabled = true;

            if (!Directory.Exists(Application.streamingAssetsPath))
                GUI.enabled = false;

            if (GUILayout.Button("清理"))
            {
                File.Delete(Application.streamingAssetsPath + ".meta");
                Directory.Delete(Application.streamingAssetsPath, true);
                AssetDatabase.Refresh();
            }
        }

        public static void Collect(string path, Dictionary<string, List<string>> rawResources)
        {
            if (path.ToLower().EndsWith(EditorUtil.baseSourceExt))
                return;

            PickResources(path, rawResources);
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
                Collect(directory, rawResources);
        }

        public static void PickResources(string path, Dictionary<string, List<string>> rawResources)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            if (fileInfos.Length <= 0)
                return;

            string sealedPath = path.EndsWith(EditorUtil.rawResourceFolder) ? EditorUtil.abRoot + EditorUtil.preLoadExt + EditorUtil.abExtension : GetBundleName(path);
            List<string> items = new List<string>();
            foreach (FileInfo fileInfo in fileInfos)
            {
                string file = fileInfo.FullName;
                if (IgnoreFile(file))
                    continue;

                string asset = "Assets" + file.Substring(Application.dataPath.Length).Replace('\\', '/');
                items.Add(asset);
            }

            if (items.Count <= 0)
                return;

            rawResources.Add(sealedPath, items);
            if (!path.EndsWith(EditorUtil.rawResourceFolder))
                pkLog += sealedPath + "(<color=#FF6EC7>" + path.Split(new[] { EditorUtil.rawResourceFolder + "\\" }, StringSplitOptions.None)[1] + "</color>)\n";
        }

        public static void Packgae(BuildTarget buildTarget)
        {
            string assemblyName = EditorUtil.devPath.Substring("Assets/".Length);
            string path;
            if (EditorUtil.devPath == string.Empty || !Directory.Exists(path = Application.dataPath + "/" + assemblyName + "/" + EditorUtil.rawResourceFolder))
            {
                CiriDebugger.LogError("打包失败，开发目录缺失！");
                return;
            }

            string compilationTemp = Environment.CurrentDirectory.Replace("\\", "/") + "/Temp/Cirilla";
            ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings();
            scriptCompilationSettings.group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            scriptCompilationSettings.target = buildTarget;
            scriptCompilationSettings.options = ScriptCompilationOptions.DevelopmentBuild;
            PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, compilationTemp);

            string assemblyPath = $"{path}/{assemblyName}{EditorUtil.preLoadExt}";
            if (!Directory.Exists(assemblyPath))
                Directory.CreateDirectory(assemblyPath);

            File.WriteAllBytes($"{assemblyPath}/{assemblyName}.bytes", File.ReadAllBytes(compilationTemp + $"/{assemblyName}.dll"));
            Directory.Delete(compilationTemp, true);
            AssetDatabase.Refresh();

            Dictionary<string, List<string>> rawResources = new Dictionary<string, List<string>>();
            Collect(path, rawResources);

            string buildPath = Application.streamingAssetsPath + $"/{EditorUtil.buildResourcesFolder}";
            if (Directory.Exists(buildPath))
                Directory.Delete(buildPath, true);

            Directory.CreateDirectory(buildPath);

            AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[rawResources.Count];
            int i = 0;
            foreach (KeyValuePair<string, List<string>> kv in rawResources)
            {
                assetBundleBuilds[i] = new AssetBundleBuild();
                assetBundleBuilds[i].assetBundleName = kv.Key;
                assetBundleBuilds[i].assetNames = kv.Value.ToArray();
                i ++;
            }
            BuildPipeline.BuildAssetBundles(buildPath, assetBundleBuilds, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
            foreach (AssetBundleBuild assetBundleBuild in assetBundleBuilds)
            {
                File.Delete(buildPath + $"/{assetBundleBuild.assetBundleName}.manifest");
                File.Delete(buildPath + $"/{assetBundleBuild.assetBundleName}.manifest.meta");
            }
            File.Delete(buildPath + $"/{EditorUtil.buildResourcesFolder}");
            File.Delete(buildPath + $"/{EditorUtil.buildResourcesFolder}.manifest");
            File.Delete(buildPath + $"/{EditorUtil.buildResourcesFolder}.manifest.meta");
            File.Delete(buildPath + $"/{EditorUtil.buildResourcesFolder}.meta");
            File.Delete(assemblyPath + ".meta");
            Directory.Delete(assemblyPath, true);
            if (pkLog != string.Empty)
                Debug.Log(pkLog);
            
            CreateMatchFile(buildPath);

            AssetDatabase.Refresh();
        }
        
        private static void CreateMatchFile(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfos = directoryInfo.GetFiles();

            string info = EditorUtil.version.ToString() + "\n";
            for (int i = 0; i < fileInfos.Length; i ++)
            {
                if (IgnoreFile(fileInfos[i].Name))
                    continue;

                FileStream fileStream = fileInfos[i].Open(FileMode.Open);
                info += $"{fileInfos[i].Name}|{Util.GetMD5(fileStream)}";
                if (i != fileInfos.Length - 1)
                    info += "\n";
                fileStream.Close();
            }

            EditorUtil.Write(path + $"/{EditorUtil.matchFile}", info);
        }

        private static string GetBundleName(string path)
        {
            path = path.ToLower().Replace("\\", "/");

            string bundleName = path.Split(new[] { EditorUtil.rawResourceFolder.ToLower() + "/" }, StringSplitOptions.None)[1].GetHashCode().ToString();

            if (path.EndsWith(EditorUtil.preLoadExt))
                return bundleName + EditorUtil.preLoadExt + EditorUtil.abExtension;

            if (path.EndsWith(EditorUtil.customLoadExt))
                return bundleName + EditorUtil.customLoadExt + EditorUtil.abExtension;

            return bundleName + EditorUtil.abExtension;
        }

        private static bool IgnoreFile(string file)
        {
            if (file.EndsWith(".meta") || file.EndsWith(".cs") || file.EndsWith(".dll"))
                return true;

            return false;
        }
    }
}
