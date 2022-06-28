
using System;
using Object = UnityEngine.Object;

namespace Cirilla
{
    public interface IResModule
    {
        T LoadAsset<T>(string path) where T : Object;
        void LoadAssetAsync<T>(string path, Action<T> callBack) where T : Object;
        void LoadCustom(string path);
        void LoadCustomAsync(string path);
        void UnLoadCustom(string path);
        void UnLoadAsset(Object obj);
        void Clear();
    }
}
