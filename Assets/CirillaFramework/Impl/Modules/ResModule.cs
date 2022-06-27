using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirilla
{
    public class ResModule : IResModule
    {
        private string streamingAssets = Application.streamingAssetsPath + "/" + Util.buildResourcesFolder;
        private string persistentData = Application.persistentDataPath + "/" + Util.buildResourcesFolder;

        private Dictionary<string, AssetInfo> assets;
        private Dictionary<string, AssetBundleInfo> bundles;

        public ResModule()
        {
            assets = new Dictionary<string, AssetInfo>();
#if UNITY_EDITOR
            if (Util.lazyLoad)
                return;
#endif
            bundles = new Dictionary<string, AssetBundleInfo>();
            LoadPublicRes(streamingAssets);
            LoadPublicRes(persistentData);
        }

        public T LoadAsset<T>(string path, PathBase pathBase) where T : Object
        {

#if UNITY_EDITOR
            if (Util.lazyLoad)
                return LazyLoadAsset<T>(path);
#endif
            path = path.Replace('\\', '/').ToLower();
            if (assets.TryGetValue(path, out AssetInfo assetInfo))
                return (T)assetInfo.obj;

            string bundleName = path.Contains("/") ? GetBundle(path) : Util.abRoot + Util.preLoadExt;

            if(!bundles.TryGetValue(bundleName, out AssetBundleInfo assetBundleInfo))
            {
                if (bundleName.EndsWith(Util.customLoadExt))
                {
                    CiriDebugger.LogError($"资源:{path}在custom包中，需要先预载包体:" + path.Replace("/" + Path.GetFileName(path), ""));
                    return null;
                }

                string loadPath = GetPathBase(pathBase) + "/" + bundleName + Util.abExtension;
#if !UNITY_ANDROID
                if (!File.Exists(loadPath))
                {
                    CiriDebugger.LogError($"资源:{path}所在包体不存在");
                    return null;
                }
#endif
                assetBundleInfo = new AssetBundleInfo(AssetBundle.LoadFromFile(GetPathBase(pathBase) + "/" + bundleName + Util.abExtension));
                bundles.Add(bundleName, assetBundleInfo);
            }

            foreach(string assetName in assetBundleInfo.assetBundle.GetAllAssetNames())
            {
                if (!assetName.Contains(path))
                    continue;

                assetInfo = new AssetInfo(assetBundleInfo.assetBundle.LoadAsset<T>(assetName), bundleName);
                assets.Add(path, assetInfo);
                assetBundleInfo.assetLoaded++;

                break;
            }

            return (T)assetInfo?.obj;
        }

        public void LoadAssetAsync<T>(string path, Action<T> callBack, PathBase pathBase) where T : Object
        {
#if UNITY_EDITOR
            if (Util.lazyLoad)
            {
                LazyLoadAssetAsync<T>(path, callBack);
                return;
            }
#endif
            path = path.Replace('\\', '/').ToLower();
            if (assets.TryGetValue(path, out AssetInfo assetInfo))
            {
                callBack((T)assetInfo.obj);
                return;
            }

            string bundleName = path.Contains("/") ? GetBundle(path) : Util.abRoot + Util.preLoadExt;

            if (bundles.TryGetValue(bundleName, out AssetBundleInfo assetBundleInfo))
            {
                Core.StartCoroutine(LoadAssetAsync(assetBundleInfo.assetBundle, path, (obj) => { callBack((T)obj); assets.Add(path, new AssetInfo(obj, bundleName)); assetBundleInfo.assetLoaded ++; }));
                return;
            }

            if (bundleName.EndsWith(Util.customLoadExt))
            {
                CiriDebugger.LogError($"资源:{path}在custom包中，需要先预载包体:" + path.Replace("/" + Path.GetFileName(path), ""));
                callBack(null);
                return;
            }

            string loadPath = GetPathBase(pathBase) + "/" + bundleName + Util.abExtension;
#if !UNITY_ANDROID
            if (!File.Exists(loadPath))
            {
                CiriDebugger.LogError($"资源:{path}所在包体不存在");
                callBack(null);
                return;
            }
#endif
            Core.StartCoroutine(LoadAssetBundleAsync(loadPath, (assetBundle)=>
            {
                assetBundleInfo = new AssetBundleInfo(assetBundle);
                bundles.Add(bundleName, assetBundleInfo);
                Core.StartCoroutine(LoadAssetAsync(assetBundle, path, (obj) => { callBack((T)obj); assets.Add(path, new AssetInfo(obj, bundleName)); assetBundleInfo.assetLoaded ++; }));
            }));

        }

        public void LoadCustom(string path, PathBase pathBase)
        {
#if UNITY_EDITOR
            if (Util.lazyLoad)
            {
                CiriDebugger.Log($"目前是懒加载模式,Custom包{path}被暂时搁置");
                return;
            }
#endif
            path = path.Replace('\\', '/').ToLower();
            if (!path.EndsWith(Util.customLoadExt))
                return;

            string bundleName = path.GetHashCode() + Util.customLoadExt;

            if (bundles.ContainsKey(bundleName))
                return;

            string loadPath = GetPathBase(pathBase) + "/" + bundleName + Util.abExtension;
#if !UNITY_ANDROID
            if (!File.Exists(loadPath))
            {
                CiriDebugger.LogError($"资源:{path}所在包体不存在");
                return;
            }
#endif
            bundles.Add(bundleName, new AssetBundleInfo(AssetBundle.LoadFromFile(loadPath)));
        }
        public void LoadCustomAsync(string path, PathBase pathBase)
        {
#if UNITY_EDITOR
            if (Util.lazyLoad)
            {
                CiriDebugger.Log($"目前是懒加载模式,Custom包{path}被暂时搁置");
                return;
            }
#endif
            path = path.Replace('\\', '/').ToLower();
            if (!path.EndsWith(Util.customLoadExt))
                return;

            string bundleName = path.GetHashCode() + Util.customLoadExt;

            if (bundles.ContainsKey(bundleName))
                return;

            string loadPath = GetPathBase(pathBase) + "/" + bundleName + Util.abExtension;
#if !UNITY_ANDROID
            if (!File.Exists(loadPath))
            {
                CiriDebugger.LogError($"资源:{path}所在包体不存在");
                return;
            }
#endif
            Core.StartCoroutine(LoadAssetBundleAsync(loadPath, (assetBundle) => {
                bundles.Add(bundleName, new AssetBundleInfo(assetBundle));
            }));
        }

        public void UnLoadCustom(string path, PathBase pathBase)
        {
#if UNITY_EDITOR
            if (Util.lazyLoad)
                return;
#endif
            path = path.Replace('\\', '/').ToLower();
            if (!path.EndsWith(Util.customLoadExt))
                return;

            string bundleName = path.GetHashCode() + Util.customLoadExt;

            if (!bundles.TryGetValue(bundleName, out AssetBundleInfo assetBundleInfo))
                return;

            foreach(KeyValuePair<string, AssetInfo> kv in new Dictionary<string, AssetInfo>(assets))
            {
                if (kv.Value.bundleName != bundleName)
                    continue;

                UnLoadAsset(kv.Value.obj);
                assets.Remove(kv.Key);
            }

            assetBundleInfo.assetBundle.Unload(true);
            bundles.Remove(bundleName);
        }

        public void UnLoadAsset(Object obj)
        {
            string target = string.Empty;
            foreach (KeyValuePair<string, AssetInfo> kv in assets)
            {
                if (kv.Value.obj != obj)
                    continue;

                target = kv.Key;

                break;
            }

            if (target == string.Empty)
                return;

            string bundleName = assets[target].bundleName;
            assets.Remove(target);
            if(!(obj is GameObject))
                Resources.UnloadAsset(obj);
            else Resources.UnloadUnusedAssets();

#if UNITY_EDITOR
            if (Util.lazyLoad)
                return;
#endif

            if (!bundles.TryGetValue(bundleName, out AssetBundleInfo assetBundleInfo))
                return;

            if (--assetBundleInfo.assetLoaded > 0)
                return;

            if (bundleName.EndsWith(Util.preLoadExt) || bundleName.EndsWith(Util.customLoadExt))
                return;

            assetBundleInfo.assetBundle.Unload(true);
            bundles.Remove(bundleName);
        }

        public void Clear()
        {
            foreach (AssetInfo assetInfo in assets.Values)
                if(!(assetInfo.obj is GameObject))
                    Resources.UnloadAsset(assetInfo.obj);

            assets.Clear();

#if UNITY_EDITOR
            if (Util.lazyLoad)
            {
                Resources.UnloadUnusedAssets();
                return;
            }
#endif

            Dictionary<string, AssetBundleInfo> buffer = new Dictionary<string, AssetBundleInfo>(bundles);

            foreach (KeyValuePair<string, AssetBundleInfo> kv in buffer)
            {
                if (kv.Key.EndsWith(Util.preLoadExt))
                    continue;

                kv.Value.assetBundle.Unload(true);
                bundles.Remove(kv.Key);
            }
        }
#if UNITY_EDITOR
        private T LazyLoadAsset<T>(string path) where T : Object
        {
            if (assets.TryGetValue(path, out AssetInfo assetInfo))
                return (T)assetInfo.obj;

            T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>($"{Util.devPath}/{Util.rawResourceFolder}/{path}");
            if (obj == null)
                return null;

            assets.Add(path, new AssetInfo(obj, ""));

            return obj;
        }

        private void LazyLoadAssetAsync<T>(string path, Action<T> callBack) where T : Object
        {
            if (assets.TryGetValue(path, out AssetInfo assetInfo))
            {
                callBack((T)assetInfo.obj);
                return;
            }

            T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>($"{Util.devPath}/{Util.rawResourceFolder}/{path}");
            callBack(obj);
            if (obj == null)
                return;

            assets.Add(path, new AssetInfo(obj, ""));
        }
#endif
        private IEnumerator LoadAssetBundleAsync(string path, Action<AssetBundle> callBack)
        {
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(path);
            yield return assetBundleCreateRequest;

            AssetBundle assetBundle = assetBundleCreateRequest.assetBundle;
            if (assetBundle == null)
            {
                CiriDebugger.Log("AB包异步加载失败:" + path);
                yield break;
            }

            callBack(assetBundle);
        }

        private IEnumerator LoadAssetAsync(AssetBundle assetBundle, string path, Action<Object> callBack)
        {
            string[] assetNames = assetBundle.GetAllAssetNames();

            string result = string.Empty;
            for (int i = 0; i < assetNames.Length; i ++)
            {
                if (!assetNames[i].Contains(path))
                    continue;
                result = assetNames[i];
                break;
            }

            AssetBundleRequest assetBundleRequest = assetBundle.LoadAssetAsync(result);
            yield return assetBundleRequest;

            Object obj = assetBundleRequest.asset;
            if (obj == null)
            {
                CiriDebugger.Log("资源异步加载失败:" + path);
                yield break;
            }

            callBack(obj);
        }

        private string GetBundle(string path)
        {
            string dirName = path.Replace("/" + Path.GetFileName(path), "");

            if (dirName.EndsWith(Util.preLoadExt))
                return dirName.GetHashCode() + Util.preLoadExt;

            if (dirName.EndsWith(Util.customLoadExt))
                return dirName.GetHashCode() + Util.customLoadExt;

            return dirName.GetHashCode().ToString();
        }

        private string GetPathBase(PathBase pathBase) => pathBase switch
        {
                PathBase.StreamingAssetsPath => streamingAssets,
                PathBase.PersistentDataPath => persistentData,
                _ => null
        };

        private void LoadPublicRes(string path)
        {
            if (!Directory.Exists(path))
                return;

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.Extension != Util.abExtension)
                    continue;

                string bundleName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                if (!bundleName.EndsWith(Util.preLoadExt))
                    continue;

                AssetBundle assetBundle = AssetBundle.LoadFromFile(fileInfo.FullName);
                bundles.Add(bundleName, new AssetBundleInfo(assetBundle));
            }
        }
    }
}