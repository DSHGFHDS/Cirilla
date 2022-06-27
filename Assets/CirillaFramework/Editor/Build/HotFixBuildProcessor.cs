using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;

#if UNITY_ANDROID
using UnityEditor.Android;
#endif

namespace Cirilla.CEditor
{
    public class HotFixBuildProcessor : IFilterBuildAssemblies, IUnityLinkerProcessor
#if UNITY_ANDROID
        , IPostGenerateGradleAndroidProject
#else
    , IPostprocessBuildWithReport
#endif
    {
        public int callbackOrder { get; }

        [Serializable]
        public class ScriptingAssemblies
        {
            public List<string> names;
            public List<int> types;
        }

        public string[] OnFilterAssemblies(BuildOptions buildOptions, string[] assemblies)
        {
            if (EditorUtil.devPath == string.Empty)
                return assemblies;

            string dllName = EditorUtil.devPath.Substring("Assets/".Length) + ".dll";
            List<string> result = new List<string>();
            for (int i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].EndsWith(dllName))
                    continue;

                result.Add(assemblies[i]);
            }
            return result.ToArray();
        }

#if UNITY_ANDROID
        public void OnPostGenerateGradleAndroidProject(string path) => AddBackHotFixAssembliesToJson(path);
#else
        public void OnPostprocessBuild(BuildReport report) => AddBackHotFixAssembliesToJson(report.summary.outputPath);
#endif

        public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data) => string.Empty;

        public void OnPostBuildPlayerScriptDLLs(BuildReport report) {
        }

        public void OnBeforeRun(BuildReport report, UnityLinkerBuildPipelineData data) {
        }

        public void OnAfterRun(BuildReport report, UnityLinkerBuildPipelineData data) {
        }

        private void AddBackHotFixAssembliesToJson(string path)
        {
            if (EditorUtil.devPath == string.Empty)
                return;

            string dllName = EditorUtil.devPath.Substring("Assets/".Length) + ".dll";
            string[] jsonFiles = Directory.GetFiles(Path.GetDirectoryName(path), "ScriptingAssemblies.json", SearchOption.AllDirectories);

            foreach (string file in jsonFiles)
            {
                string content = File.ReadAllText(file);
                ScriptingAssemblies scriptingAssemblies = JsonUtility.FromJson<ScriptingAssemblies>(content);
                scriptingAssemblies.names.Add(dllName);
                scriptingAssemblies.types.Add(16);

                content = JsonUtility.ToJson(scriptingAssemblies);
                File.WriteAllText(file, content);
            }
        }
    }
}
