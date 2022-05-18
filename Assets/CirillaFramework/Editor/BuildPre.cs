


using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Cirilla.CEditor
{
    class BuildPre : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            string dllName = $"{ EditorUtil.devPath.Substring("Assets/".Length) }.dll";
            string dllPath = Environment.CurrentDirectory.Replace("\\", "/") + $"/Library/ScriptAssemblies/{dllName}";
            if (!File.Exists(dllPath))
            {
                CiriDebugger.LogError(dllPath + "打包缺失：" + dllPath);
                return;
            }

            byte[] dllBytes = File.ReadAllBytes(dllPath);

            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);

            File.WriteAllBytes(Application.streamingAssetsPath + $"/{dllName}", dllBytes);
        }
    }
}
