 
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

        public static void Packgae(BuildTarget buildTarget)
        {
            pkLog = string.Empty;
            List<List<AssetBundleBuild>> resBuffer = new List<List<AssetBundleBuild>>();

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
            Collect(path, resBuffer);

            string buildPath = Application.streamingAssetsPath + $"/{EditorUtil.buildResourcesFolder}";
            if (Directory.Exists(buildPath))
                Directory.Delete(buildPath, true);

            Directory.CreateDirectory(buildPath);
            foreach (List<AssetBundleBuild> assetBundleBuilds in resBuffer)
            {
                BuildPipeline.BuildAssetBundles(buildPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
                foreach (AssetBundleBuild assetBundleBuild in assetBundleBuilds)
                {
                    File.Delete(buildPath + $"/{assetBundleBuild.assetBundleName}.manifest");
                    File.Delete(buildPath + $"/{assetBundleBuild.assetBundleName}.manifest.meta");
                }
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

        private static void Collect(string path, List<List<AssetBundleBuild>> resBuffer)
        {
            if (path.ToLower().EndsWith(EditorUtil.baseSourceExt))
                return;

            PickResources(path, resBuffer);
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
                Collect(directory, resBuffer);
        }

        private static void PickResources(string path, List<List<AssetBundleBuild>> resBuffer)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            if (fileInfos.Length <= 0)
                return;

            List<string> items = new List<string>();
            foreach (FileInfo fileInfo in fileInfos)
            {
                string file = fileInfo.FullName;
                if (IgnoreFile(file))
                    continue;

                string asset = "Assets" + file.Substring(Application.dataPath.Length).Replace('\\', '/');

                if (!items.Contains(asset))
                    items.Add(asset);

                string[] dependences = AssetDatabase.GetDependencies(asset, true);
                foreach (string dependence in dependences)
                {
                    if (IgnoreFile(dependence) || asset == dependence)
                        continue;

                    if (items.Contains(dependence))
                        continue;

                    items.Add(dependence);
                }
            }

            if (items.Count <= 0)
                return;

            AssetBundleBuild resultBuild = new AssetBundleBuild();
            resultBuild.assetBundleName = path.EndsWith(EditorUtil.rawResourceFolder) ? EditorUtil.abRoot + EditorUtil.preLoadExt + EditorUtil.abExtension : GetBundleName(path);
            resultBuild.assetNames = items.ToArray();

            if (!path.EndsWith(EditorUtil.rawResourceFolder))
                pkLog += resultBuild.assetBundleName + "(<color=#FF6EC7>" + path.Split(new[] { EditorUtil.rawResourceFolder + "\\" }, StringSplitOptions.None)[1] + "</color>)\n";

            foreach(List<AssetBundleBuild> assetBundleBuilds in resBuffer)
            {
                for(int i = 0; i < assetBundleBuilds.Count; i ++)
                {
                    if (!assetsContains(resultBuild.assetNames, assetBundleBuilds[i].assetNames))
                    {
                        if (i != assetBundleBuilds.Count - 1)
                            continue;

                        assetBundleBuilds.Add(resultBuild);
                        return;
                    }
                    break;
                }
            }

            resBuffer.Add(new List<AssetBundleBuild> { resultBuild });
        }

        private static bool assetsContains(string[] originAssetNames, string[] targetAssetNames)
        {
            foreach(string ori in originAssetNames)
            {
                foreach(string target in targetAssetNames)
                {
                    if (ori != target)
                        continue;

                    return true;
                }
            }

            return false;
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
