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
        private static readonly string resourcesPath = Application.streamingAssetsPath + "/" + Util.platform;

        private Dictionary<string, AssetInfo> assets;
        private Dictionary<string, AssetBundleInfo> bundles;
        public ResModule()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            assets = new Dictionary<string, AssetInfo>();
            bundles = new Dictionary<string, AssetBundleInfo>();
            if (!Directory.Exists(resourcesPath))
                return;

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
                    CiriDebugger.LogError($"资源:{path}在custom包中，需要先预载包体:" + path.Replace("/" + Path.GetFileName(path), ""));
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

            return (T)assetInfo?.obj;
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
                Core.StartCoroutine(LoadAssetAsync(assetBundleInfo.assetBundle, path, (obj) => { callBack((T)obj); assets.Add(path, new AssetInfo(obj, bundleName)); assetBundleInfo.assetLoaded ++; }));
                return;
            }

            if (bundleName.EndsWith(Util.customLoadExt))
            {
                CiriDebugger.LogError($"资源:{path}在custom包中，需要先预载包体:" + path.Replace("/" + Path.GetFileName(path), ""));
                return;
            }

            Core.StartCoroutine(LoadAssetBundleAsync(resourcesPath + "/" + bundleName + Util.abExtension, (assetBundle)=>
            {
                assetBundleInfo = new AssetBundleInfo(assetBundle);
                bundles.Add(bundleName, assetBundleInfo);
                Core.StartCoroutine(LoadAssetAsync(assetBundle, path, (obj) => { callBack((T)obj); assets.Add(path, new AssetInfo(obj, bundleName)); assetBundleInfo.assetLoaded ++; }));
            }));
        }

        public void LoadCustom(string path)
        {
            path = path.Replace('\\', '/').ToLower();
            if (!path.EndsWith(Util.customLoadExt))
                return;

            string bundleName = path.GetHashCode() + Util.customLoadExt;

            if (bundles.ContainsKey(bundleName))
                return;

            bundles.Add(bundleName, new AssetBundleInfo(AssetBundle.LoadFromFile(resourcesPath + "/" + bundleName + Util.abExtension)));
        }
        public void LoadCustomAsync(string path)
        {
            path = path.Replace('\\', '/').ToLower();
            if (!path.EndsWith(Util.customLoadExt))
                return;

            string bundleName = path.GetHashCode() + Util.customLoadExt;

            if (bundles.ContainsKey(bundleName))
                return;

            Core.StartCoroutine(LoadAssetBundleAsync(resourcesPath + "/" + bundleName + Util.abExtension, (assetBundle) => {
                bundles.Add(bundleName, new AssetBundleInfo(assetBundle));
            }));
        }

        public void UnLoadCustom(string path)
        {
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
            Resources.UnloadUnusedAssets();
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
            
            if (!bundles.TryGetValue(bundleName, out AssetBundleInfo assetBundleInfo))
                return;

            if (--assetBundleInfo.assetLoaded > 0)
                return;

            if (bundleName.EndsWith(Util.preLoadExt) || bundleName.EndsWith(Util.customLoadExt))
                return;

            assetBundleInfo.assetBundle.Unload(true);
            bundles.Remove(bundleName);
            Resources.UnloadUnusedAssets();
        }

        public void Clear()
        {
            foreach(AssetInfo assetInfo in assets.Values)
                if(!(assetInfo.obj is GameObject))
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

            Resources.UnloadUnusedAssets();
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
    }
}