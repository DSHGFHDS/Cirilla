using UnityEditor;
using UnityEngine;
using System.IO;

namespace Cirilla
{
    public class CreateDataPanel
    {
        private static string fileName = "NewConfig";
        [MenuItem("Assets/Cirilla/创建配置文件")]
        public static void CreateScriptObject()
        {
            string path;
            Object[] arr = Selection.GetFiltered<Object>(SelectionMode.TopLevel);

            if (arr.Length > 0)
            {
                path = AssetDatabase.GetAssetPath(arr[0]);
                if (!Directory.Exists(path))
                {
                    string[] splitBuffer = path.Split('/');
                    path = path.Substring(0, path.Length - splitBuffer[splitBuffer.Length - 1].Length - 1);
                }
            }
            else path = "Assets";

            string resultPath;
            int i = 0;
            while (true)
            {
                if (i == 0) resultPath = path + "/" + fileName + ".asset";
                else resultPath = path + "/" + fileName + "(" + i + ").asset";

                if (File.Exists(resultPath))
                {
                    i++;
                    continue;
                }
                break;
            }

            DataPanel configData = ScriptableObject.CreateInstance<DataPanel>();

            AssetDatabase.CreateAsset(configData, resultPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = configData;
        }
    }
}