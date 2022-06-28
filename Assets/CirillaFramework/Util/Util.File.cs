
using System;
using System.IO;
using System.Text;
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

        public static string GetMD5(FileStream fileStream)
        {

            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fileStream);

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
                stringBuilder.Append(retVal[i].ToString("x2"));

            return stringBuilder.ToString();
        }
    }
}