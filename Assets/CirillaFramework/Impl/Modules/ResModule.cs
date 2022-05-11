using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirilla
{
    public class ResModule : IRes
    {
        private static readonly string resourcesPath = Application.streamingAssetsPath + "/" + Util.platform;

        private Dictionary<string, AssetInfo> assets;
        private Dictionary<string, AssetBundleInfo> bundles;
        public ResModule()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            assets = new Dictionary<string, AssetInfo>();
            bundles = new Dictionary<string, AssetBundleInfo>();
            DirectoryInfo directoryInfo = new DirectoryInfo(resourcesPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            foreach(FileInfo fileInfo in fileInfos)
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

        public T LoadAsset<T>(string path) where T : Object
        {
            path = path.Replace('\\', '/').ToLower();
            if (assets.TryGetValue(path, out AssetInfo assetInfo))
                return (T)assetInfo.obj;

            string bundleName = path.Contains("/") ? GetBundle(path) : Util.abRoot + Util.preLoadExt;

            if(!bundles.TryGetValue(bundleName, out AssetBundleInfo assetBundleInfo))
            {
                if (bundleName.EndsWith(Util.customLoadExt))
                {
                    CiriDebugger.LogError("该资源在custom包中，需要先进行包体预载:" + path);
                    return null;
                }

                assetBundleInfo = new AssetBundleInfo(AssetBundle.LoadFromFile(resourcesPath + "/" + bundleName + Util.abExtension));
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

            return (T)assetInfo.obj;
        }

        public void LoadAssetAsync<T>(string path, Action<T> callBack) where T : Object
        {
            path = path.Replace('\\', '/').ToLower();
            if (assets.TryGetValue(path, out AssetInfo assetInfo))
            {
                callBack((T)assetInfo.obj);
                return;
            }

            string bundleName = path.Contains("/") ? GetBundle(path) : Util.abRoot + Util.preLoadExt;

            if (bundles.TryGetValue(bundleName, out AssetBundleInfo assetBundleInfo))
            {
                CirillaCore.StartCoroutine(LoadAssetAsync(assetBundleInfo.assetBundle, path, (obj) => { callBack((T)obj); assets.Add(path, new AssetInfo(obj, bundleName)); assetBundleInfo.assetLoaded ++; }));
                return;
            }

            if (bundleName.EndsWith(Util.customLoadExt))
            {
                CiriDebugger.LogError("该资源在custom包中，需要先进行包体预载:" + path);
                return;
            }

            CirillaCore.StartCoroutine(LoadAssetBundleAsync(resourcesPath + "/" + bundleName + Util.abExtension, (assetBundle)=>
            {
                assetBundleInfo = new AssetBundleInfo(assetBundle);
                bundles.Add(bundleName, assetBundleInfo);
                CirillaCore.StartCoroutine(LoadAssetAsync(assetBundle, path, (obj) => { callBack((T)obj); assets.Add(path, new AssetInfo(obj, bundleName)); assetBundleInfo.assetLoaded ++; }));
            }));
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

            assets.Remove(target);
            Resources.UnloadAsset(assets[target].obj);

            if (!bundles.TryGetValue(assets[target].bundleName, out AssetBundleInfo assetBundleInfo))
                return;

            if (--assetBundleInfo.assetLoaded > 0)
                return;

            if (assets[target].bundleName.EndsWith(Util.preLoadExt))
                return;

            assetBundleInfo.assetBundle.Unload(true);
            bundles.Remove(assets[target].bundleName);
        }

        public void Clear()
        {
            foreach(AssetInfo assetInfo in assets.Values)
                Resources.UnloadAsset(assetInfo.obj);

            assets.Clear();

            Dictionary<string, AssetBundleInfo> buffer = new Dictionary<string, AssetBundleInfo>(bundles);

            foreach (KeyValuePair<string, AssetBundleInfo> kv in buffer)
            {
                if (kv.Key.EndsWith(Util.preLoadExt))
                    continue;

                kv.Value.assetBundle.Unload(true);
                bundles.Remove(kv.Key);
            }
        }

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
            int i;
            for (i = 0; i < assetNames.Length; i ++)
            {
                if (!assetNames[i].Contains(path))
                    continue;
                break;
            }

            AssetBundleRequest assetBundleRequest = assetBundle.LoadAssetAsync(assetNames[i]);
            yield return assetBundleRequest;

            Object obj = assetBundleRequest.asset;
            if (obj == null)
            {
                CiriDebugger.Log("资源加载失败:" + path);
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
    }
}