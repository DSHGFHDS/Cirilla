
using System;
using Object = UnityEngine.Object;

namespace Cirilla
{
    public interface IRes
    {
        T LoadAsset<T>(string path) where T : Object;
        void LoadAssetAsync<T>(string path, Action<T> callBack) where T : Object;
        void UnLoadAsset(Object obj);
        void Clear();
    }
}
