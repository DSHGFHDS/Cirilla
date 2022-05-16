


using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Cirilla.CEditor
{
    class BuildPost : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPostprocessBuild(BuildReport report)
        {
            string dllName = $"{ EditorUtil.devPath.Substring("Assets/".Length) }.dll";
            string dllPath = Environment.CurrentDirectory.Replace("\\", "/") + $"/Library/ScriptAssemblies/{dllName}";
            if (!File.Exists(dllPath))
            {
                CiriDebugger.LogError(dllPath + "打包缺失：" + dllPath);
                return;
            }
            byte[] dllBytes = File.ReadAllBytes(dllPath);

            string dataPath = $"{Path.GetDirectoryName(report.summary.outputPath)}\\{Application.productName}_Data\\";
            dllPath = $"{dataPath}StreamingAssets\\";
            if(!Directory.Exists(dllPath))
                Directory.CreateDirectory(dllPath);

            File.WriteAllBytes($"{dllPath}{dllName}", dllBytes);

            dllPath = $"{dataPath}Managed\\{dllName}";
            File.Delete(dllPath);
        }
    }
}
