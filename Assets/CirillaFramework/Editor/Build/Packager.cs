﻿
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
        private static BuildTarget selectedBuildTarget = BuildTarget.StandaloneWindows64;

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

            EditorGUILayout.HelpBox("选择平台进行资源打包", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("平台", new[] { GUILayout.Height(20), GUILayout.Width(40) });
            selectedBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(selectedBuildTarget);
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
            List<AssetBundleBuild> resBuffer = new List<AssetBundleBuild>();

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
            BuildPipeline.BuildAssetBundles(buildPath, resBuffer.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
            File.Delete(assemblyPath + ".meta");
            Directory.Delete(assemblyPath, true);
            AssetDatabase.Refresh();
        }

        private static void Collect(string path, List<AssetBundleBuild> resBuffer)
        {
            PickResources(path, resBuffer);
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
                Collect(directory, resBuffer);
        }

        private static void PickResources(string path, List<AssetBundleBuild> resBuffer)
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

            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetBundleName = path.EndsWith(EditorUtil.rawResourceFolder) ? EditorUtil.abRoot + EditorUtil.preLoadExt + EditorUtil.abExtension : GetBundleName(path);
            assetBundleBuild.assetNames = items.ToArray();
            resBuffer.Add(assetBundleBuild);
        }

        public static string GetBundleName(string path)
        {
            path = path.ToLower().Replace("\\", "/");
            string bundleName = path.Split(new[] { EditorUtil.rawResourceFolder.ToLower() + "/" }, System.StringSplitOptions.None)[1].GetHashCode().ToString();

            if (path.EndsWith(EditorUtil.preLoadExt))
                return bundleName + EditorUtil.preLoadExt + EditorUtil.abExtension;

            if (path.EndsWith(EditorUtil.customLoadExt))
                return bundleName + EditorUtil.customLoadExt + EditorUtil.abExtension;

            return bundleName + EditorUtil.abExtension;
        }

        private static bool IgnoreFile(string file)
        {
            string[] buffer = file.Split('.');

            if (buffer.Length < 2)
                return true;

            switch (buffer[buffer.Length-1])
            {
                case "meta":
                    return true;
                case "cs":
                    return true;
                case "dll":
                    return true;
            }

            return false;
        }
    }
}