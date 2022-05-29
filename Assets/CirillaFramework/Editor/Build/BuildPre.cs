
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cirilla.CEditor
{

    [InitializeOnLoad]
    public class BuildPre
    {
        static BuildPre() => BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuild);

        public static void OnBuild(BuildPlayerOptions options)
        {
            Packager.Packgae(options.target);
            BuildPipeline.BuildPlayer(options);
            File.Delete(Application.streamingAssetsPath + ".meta");
            Directory.Delete(Application.streamingAssetsPath, true);
            AssetDatabase.Refresh();
        }
    }
}
