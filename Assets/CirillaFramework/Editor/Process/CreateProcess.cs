using UnityEditor;
using UnityEngine;
using System.IO;

namespace Cirilla
{
    public class CreateProcess
    {
        private static string fileName = "NewProcess";
        [MenuItem("Assets/Cirilla/创建流程文件")]
        public static void CreateProcessFile()
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
                if (i == 0) resultPath = path + "/" + fileName + ".cs";
                else resultPath = path + "/" + fileName + "(" + i + ").cs";

                if (File.Exists(resultPath))
                {
                    i++;
                    continue;
                }
                break;
            }

            string code =
            "using Cirilla;" + "\n" +
            "namespace GameLogic" + "\n" +
            "{" + "\n" +
            $"    public class {Path.GetFileNameWithoutExtension(resultPath)} : {typeof(AProcessBase).Name}" +"\n" +
            "    {" + "\n" +
            "        #region 流程初始化与释放" + "\n" +
            "        public override void Init()" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "        #endregion" + "\n" +
            "        #region 流程往返" + "\n" +
            "        public override void OnEnter(params object[] args)" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "        public override void OnExit()" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "        #endregion" + "\n" +
            "        #region 心跳帧" + "\n" +
            "        public override void OnInputUpdate()" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "        public override void OnLogicUpdatePre()" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "        public override void OnLogicUpdatePost()" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "        public override void OnPhysicUpdate()" + "\n" +
            "        {" + "\n" +
            "        }" + "\n" +
            "        #endregion" + "\n" +
            "    }" + "\n" +
            "}" + "\n";

            File.WriteAllText(resultPath, code);
            AssetDatabase.Refresh();
        }
    }
}