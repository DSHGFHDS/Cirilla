
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
            Directory.Delete(Application.streamingAssetsPath, true);
            File.Delete(Application.streamingAssetsPath + ".meta");
        }
    }
}
