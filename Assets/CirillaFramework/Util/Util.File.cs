
using System;
using System.IO;
using UnityEditor.Compilation;
using UnityEngine;

namespace Cirilla
{
    public partial class Util
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
        public static string SearchFileByType(Type type)
        {
            string[] files = Directory.GetFiles(Application.dataPath, ".", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Contains(type.Name+".cs"))
                    continue;

                return files[i].Replace("/", "\\");
            }

            return null;
        }
    }
}