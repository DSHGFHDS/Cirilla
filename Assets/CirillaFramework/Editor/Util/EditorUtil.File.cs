
using System;
using System.IO;
using UnityEngine;

namespace Cirilla.CEditor
{
    public partial class EditorUtil
    {
        public static string SearchFileByName(string fileName)
        {
            string[] files = Directory.GetFiles(Application.dataPath, ".", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Contains(fileName))
                    continue;

                return files[i].Replace("/", "\\");
            }

            return null;
        }
        public static string SearchFileByType(string path, Type type)
        {
            string[] files = Directory.GetFiles(path, ".", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Contains(type.Name+".cs"))
                    continue;

                return files[i].Replace("/", "\\");
            }

            return null;
        }

        public static void Write(string path, string text)
        {
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.Write(text);
            streamWriter.Close();
        }
    }
}