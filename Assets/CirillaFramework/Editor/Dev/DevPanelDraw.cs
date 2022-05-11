using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Cirilla.CEditor
{
    public class DevPanelDraw : EditorWindow
    {
        private static readonly string[] replaceCode = { "dhuash78awdajadada", "dd5564564awdwadw" };
        private static readonly string assemblyDef =
            "{" + "\n" +
            "    \"name\": \"" + replaceCode[0] + "\"," + "\n" +
            "    \"rootNamespace\": \"\"," + "\n" +
            "    \"references\": [\"GUID:"+ replaceCode[1] + "\"]," + "\n" +
            "    \"includePlatforms\": []," + "\n" +
            "    \"excludePlatforms\": []," + "\n" +
            "    \"allowUnsafeCode\": false," + "\n" +
            "    \"overrideReferences\": false," + "\n" +
            "    \"precompiledReferences\": []," + "\n" +
            "    \"autoReferenced\": true," + "\n" +
            "    \"defineConstraints\": []," + "\n" +
            "    \"versionDefines\": []," + "\n" +
            "    \"noEngineReferences\": false" + "\n" +
            "}";

        [MenuItem("Cirilla/�������ñ�")]
        private static void Open()
        {
            GetWindow<DevPanelDraw>("�������ñ�").Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("��Ŀ����ʱ�����صĳ���", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            string folder = string.Empty;
            if (GUILayout.Button("ѡ��"))
            {
                string fullFolder = EditorUtil.devPath == string.Empty ? Application.dataPath : Application.dataPath + "/" + EditorUtil.devPath.Substring("Assets/".Length);
                if (!Directory.Exists(fullFolder))
                {
                    EditorUtil.devPath = string.Empty;
                    fullFolder = Application.dataPath;
                }

                fullFolder = EditorUtility.OpenFolderPanel("ѡ����Ŀ����Ŀ¼", fullFolder, "");
                if (fullFolder == string.Empty)
                    goto endHorizontal;

                if (!fullFolder.Contains("Assets") || fullFolder.Contains("Editor") || fullFolder.Contains("Plugins") || fullFolder.Contains("Resources") || fullFolder.Contains("StreamingAssets")
                     || fullFolder.Contains("Gizmos") || fullFolder.Contains("Editor Default Resources") || fullFolder.Contains("CirillaFramework"))
                {
                    CiriDebugger.LogError("��֧�����ø�Ŀ¼Ϊ����Ŀ¼");
                    goto endHorizontal;
                }

                folder = fullFolder.Substring(Application.dataPath.Length - "Assets".Length);
                if(folder == "Assets")
                {
                    CiriDebugger.LogError("��ѡ��Ŀ¼");
                    goto endHorizontal;
                }

                EditorUtil.devPath = folder;
                CheckFile(fullFolder);
            }
            GUI.enabled = false;
            EditorGUILayout.TextField(EditorUtil.devPath);
            GUI.enabled = true;

        endHorizontal:
            EditorGUILayout.EndHorizontal();
        }

        private void CheckFile(string path)
        {
            List<string> directories = new List<string>(Directory.GetDirectories(path));

            if (!directories.Contains(EditorUtil.resourceFolder))
                Directory.CreateDirectory(path+"/"+ EditorUtil.resourceFolder);

            string processFile = Application.dataPath + "/" + EditorUtil.devPath.Substring("Assets/".Length) + "/ProcessType.cs";
            if (!File.Exists(processFile))
                ProcessPanelDraw.Write(processFile, null);

            AssetDatabase.Refresh();

            string assemblyName = Path.GetFileName(path);

            if (File.Exists(path + "/" + assemblyName + ".asmdef"))
                return;

            string writePath = path + "/" + assemblyName + ".asmdef";
            string text = assemblyDef.Replace(replaceCode[0], assemblyName).Replace(replaceCode[1], AssetDatabase.GUIDFromAssetPath("Assets/CirillaFramework/CirilaFramework.asmdef").ToString());
            Util.Write(writePath, text);
            AssetDatabase.Refresh();
        }
    }
}