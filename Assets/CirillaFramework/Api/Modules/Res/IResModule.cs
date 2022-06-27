
using System;
using Object = UnityEngine.Object;

namespace Cirilla
{
    public interface IResModule
    {
        T LoadAsset<T>(string path, PathBase pathBase = PathBase.StreamingAssetsPath) where T : Object;
        void LoadAssetAsync<T>(string path, Action<T> callBack, PathBase pathBase = PathBase.StreamingAssetsPath) where T : Object;
        void LoadCustom(string path, PathBase pathBase = PathBase.StreamingAssetsPath);
        void LoadCustomAsync(string path, PathBase pathBase = PathBase.StreamingAssetsPath);
        void UnLoadCustom(string path, PathBase pathBase = PathBase.StreamingAssetsPath);
        void UnLoadAsset(Object obj);
        void Clear();
    }
}
