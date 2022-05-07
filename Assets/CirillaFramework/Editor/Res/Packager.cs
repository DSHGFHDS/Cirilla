
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cirilla.CEditor
{
    public class Packager : Editor
    {
        private static string devResPath = Application.dataPath.Replace('/', '\\') + "\\GameLogic\\Res";
        private static string buildPath = Directory.GetCurrentDirectory() + "\\StreamingAssets\\Res";
        private static List<AssetBundleBuild> resBuffer = new List<AssetBundleBuild>();
        [MenuItem("Cirilla/资源打包")]
        private static void Packgae()
        {
            resBuffer.Clear();
            PackageRoot();
            PackageNodes();
            
            if (Directory.Exists(buildPath))
                Directory.Delete(buildPath, true);

            Directory.CreateDirectory(buildPath);

            BuildPipeline.BuildAssetBundles(buildPath, resBuffer.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
        }
        
        private static void PackageRoot()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(devResPath);
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
            assetBundleBuild.assetBundleName = "TopRoot";
            assetBundleBuild.assetNames = items.ToArray();
            resBuffer.Add(assetBundleBuild);
        }
        
        private static void PackageNodes()
        {
            string[] directories = Directory.GetDirectories(devResPath);
            foreach (string directory in directories)
            {
                List<string> items = new List<string>();
                string[] files = Directory.GetFiles(directory, ".", SearchOption.AllDirectories);
                foreach (string file in files)
                {
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
                    continue;

                AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
                assetBundleBuild.assetBundleName = directory.Substring(devResPath.Length+1);
                assetBundleBuild.assetNames = items.ToArray();
                resBuffer.Add(assetBundleBuild);
            }
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
