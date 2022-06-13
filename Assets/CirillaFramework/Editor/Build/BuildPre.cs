
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
            if (EditorUtil.devPath == string.Empty || !Directory.Exists(Application.dataPath + "/" + EditorUtil.devPath.Substring("Assets/".Length))
                || !File.Exists(Application.dataPath + "/" + EditorUtil.devPath.Substring("Assets/".Length) + "/" + EditorUtil.devPath.Substring("Assets/".Length) + ".asmdef")
                || !Directory.Exists(Application.dataPath + "/" + EditorUtil.devPath.Substring("Assets/".Length) + "/" + EditorUtil.rawResourceFolder))
            {
                CiriDebugger.LogError("Build失败，开发目录缺失或不完整！");
                return;
            }

            Packager.Packgae(options.target);
            BuildPipeline.BuildPlayer(options);
            File.Delete(Application.streamingAssetsPath + ".meta");
            Directory.Delete(Application.streamingAssetsPath, true);
            AssetDatabase.Refresh();
        }
    }
}
