
using System;
using Object = UnityEngine.Object;

namespace Cirilla
{
    public interface IRes
    {
        T LoadAsset<T>(string packageName, string resourceName) where T : Object;
        void LoadAssetAsync<T>(string packageName, string resourceName, Action<T> callBack) where T : Object;
        T LoadAsset<T>(string resourcePath) where T : Object;
        void LoadAssetAsync<T>(string resourcePath, Action<T> callBack) where T : Object;
        string[] GetAllAssetNames(string packageName);
        void LoadPackage(string packageName, bool LoadDependencies = true);
        void UnloadPackage(string packageName);
        void UnloadAsset(string packageName, string resourceName);
        void UnloadAsset(string resourcePath);
        void Clear();
    }
}
