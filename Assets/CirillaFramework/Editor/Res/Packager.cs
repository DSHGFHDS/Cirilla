
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cirilla.CEditor
{
    public class Packager : Editor
    {
        private static List<AssetBundleBuild> resBuffer = new List<AssetBundleBuild>();
        [MenuItem("Cirilla/资源打包")]
        public static void Packgae()
        {
            resBuffer.Clear();

            string path;
            if (EditorUtil.devPath == string.Empty || !Directory.Exists(path = Application.dataPath + "/" + EditorUtil.devPath.Substring("Assets/".Length) + "/" + EditorUtil.resourceFolder))
            {
                CiriDebugger.LogError("打包失败，开发目录缺失！");
                return;
            }

            path = path.Replace('/', '\\');
            Collect(path);

            string buildPath = Application.streamingAssetsPath + $"\\{EditorUtil.platform}\\";
            if (Directory.Exists(buildPath))
                Directory.Delete(buildPath, true);

            Directory.CreateDirectory(buildPath);

            BuildTarget buildTarget = BuildTarget.StandaloneWindows;
            switch(EditorUtil.platform)
            {
                case RuntimePlatform.PCx64:
                    buildTarget = BuildTarget.StandaloneWindows64;
                    break;
                case RuntimePlatform.PC:
                    buildTarget = BuildTarget.StandaloneWindows;
                    break;
                case RuntimePlatform.Android:
                    buildTarget = BuildTarget.Android;
                    break;
                case RuntimePlatform.IOS:
                    buildTarget = BuildTarget.iOS;
                    break;
            }

            BuildPipeline.BuildAssetBundles(buildPath, resBuffer.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
        }

        private static void Collect(string path)
        {
            PickResources(path);
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
                Collect(directory);
        }

        private static void PickResources(string path)
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

                    if (!items.Contains(dependence))
                        items.Add(dependence);
                }
            }

            if (items.Count <= 0)
                return;

            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetBundleName = path.EndsWith(EditorUtil.resourceFolder) ? EditorUtil.abRoot + EditorUtil.preLoadExt + EditorUtil.abExtension : GetBundleName(path);
            assetBundleBuild.assetNames = items.ToArray();
            resBuffer.Add(assetBundleBuild);
        }

        public static string GetBundleName(string path)
        {
            path = path.ToLower().Replace("\\", "/");
            string bundleName = path.Split(new[] { EditorUtil.resourceFolder.ToLower() + "/" }, System.StringSplitOptions.None)[1].GetHashCode().ToString();

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
