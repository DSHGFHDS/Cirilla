using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirilla
{
    public class ResManager : IRes
    {
        private static readonly string resTag = "Resources|89hw8(*G*&G021oihn''qeqq";
        private static readonly string pathUrl = Application.streamingAssetsPath + "/";
        private static readonly string mainName =
#if UNITY_IOS
             "IOS";
#elif UNITY_ANDROID
             "Android";
#else
             "PC";
#endif

        private Dictionary<string, KeyValuePair<AssetBundle, Dictionary<string, Object>>> resStock;
        private AssetBundle mainPackage;
        private AssetBundleManifest maniFest;

        public ResManager() {
            resStock = new Dictionary<string, KeyValuePair<AssetBundle, Dictionary<string, Object>>>();
            resStock.Add(resTag, new KeyValuePair<AssetBundle, Dictionary<string, Object>>(null, new Dictionary<string, Object>()));
        }

        private bool LoadMainPackage()
        {
            if (mainPackage != null)
                return true;

            if ((mainPackage = AssetBundle.LoadFromFile(pathUrl + mainName)) == null)
            {
                CiriDebugger.LogError("doesn't exist:" + pathUrl + mainName);
                return false;
            }

            if ((maniFest = mainPackage.LoadAsset<AssetBundleManifest>("AssetBundleManifest")) != null)
                return true;

            CiriDebugger.LogError("doesn't exist" + pathUrl + maniFest);
            return false;
        }

        private void LoadAllDependencies(string packageName)
        {
            if (!LoadMainPackage())
                return;

            foreach (string name in maniFest.GetAllDependencies(packageName))
                LoadPackage(name, false);
        }

        public T LoadAsset<T>(string packageName, string resourceName) where T : Object
        {
            if (!resStock.TryGetValue(packageName, out KeyValuePair<AssetBundle, Dictionary<string, Object>> kv))
            {
                CiriDebugger.LogError("Unloaded:" + resourceName);
                return null;
            }

            if (kv.Value.TryGetValue(resourceName, out Object asset))
                return (T)asset;

            if (kv.Key == null)
            {
                CiriDebugger.LogError("Doesn't exist or unloaded:" + resourceName);
                return null;
            }

            asset = kv.Key.LoadAsset<T>(resourceName);
            if(asset == null)
            {
                CiriDebugger.LogError("Doesn't exist:" + resourceName);
                return null;
            }

            kv.Value.Add(resourceName, asset);

            return (T)asset;
        }

        public void LoadAssetAsync<T>(string packageName, string resourceName, Action<T> callBack) where T : Object
        {
            if (!resStock.TryGetValue(packageName, out KeyValuePair<AssetBundle, Dictionary<string, Object>> kv))
            {
                CiriDebugger.LogError("Unloaded:" + resourceName);
                return;
            }

            if (kv.Value.TryGetValue(resourceName, out Object asset))
            {
                callBack((T)asset);
                return;
            }

            if (kv.Key == null)
            {
                CiriDebugger.LogError("Doesn't exist or unloaded:" + resourceName);
                return;
            }

            CirillaCore.StartCoroutine(LoadCoroutine(packageName, resourceName, callBack));
        }

        private IEnumerator LoadCoroutine<T>(string packageName, string resourceName, Action<T> callBack) where T : Object
        {
            AssetBundleRequest request = resStock[packageName].Key.LoadAssetAsync<T>(resourceName);
            yield return request;

            if (!request.isDone)
            {
                CiriDebugger.LogError("Doesn't exist:" + resourceName);
                yield break;
            }

            resStock[packageName].Value.Add(resourceName, request.asset);
            callBack(request.asset as T);
        }

        public T LoadAsset<T>(string resourcePath) where T : Object
        {
            if (resStock[resTag].Value.TryGetValue(resourcePath, out Object asset))
                return (T)asset;

            asset = Resources.Load<T>(resourcePath);
            if (asset == null)
            {
                CiriDebugger.LogError("Doesn't exist:" + resourcePath);
                return null;
            }

            resStock[resTag].Value.Add(resourcePath, asset);

            return (T)asset;
        }

        public void LoadAssetAsync<T>(string resourcePath, Action<T> callBack) where T : Object
        {
            if (resStock[resTag].Value.TryGetValue(resourcePath, out Object asset))
            {
                callBack((T)asset);
                return;
            }

            CirillaCore.StartCoroutine(LoadCoroutine(resourcePath, callBack));
        }

        private IEnumerator LoadCoroutine<T>(string resourcePath, Action<T> callBack) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(resourcePath);
            yield return request;

            if (!request.isDone)
            {
                CiriDebugger.LogError("Doesn't exist:" + resourcePath);
                yield break;
            }

            callBack((T)request.asset);
        }

        public string[] GetAllAssetNames(string packageName)
        {
            if (!resStock.TryGetValue(packageName, out KeyValuePair<AssetBundle, Dictionary<string, Object>> kv) || kv.Key == null)
                return null;

            return resStock[packageName].Key.GetAllAssetNames();
        }

        public void LoadPackage(string packageName, bool LoadDependencies = true)
        {
            if (resStock.TryGetValue(packageName, out KeyValuePair<AssetBundle, Dictionary<string, Object>> kv) && kv.Key != null)
                return;

            AssetBundle package = AssetBundle.LoadFromFile(pathUrl + packageName);
            if (package == null)
            {
                CiriDebugger.LogError("不存在AB包:" + pathUrl + packageName);
                return;
            }

            if(kv.Value == null)
                resStock.Add(packageName, new KeyValuePair<AssetBundle, Dictionary<string, Object>>(package, new Dictionary<string, Object>()));
            else
                kv = new KeyValuePair<AssetBundle, Dictionary<string, Object>>(package, kv.Value);

            if(!LoadDependencies)
                return;

            LoadAllDependencies(packageName);
        }

        public void UnloadPackage(string packageName)
        {
            if (!resStock.TryGetValue(packageName, out KeyValuePair<AssetBundle, Dictionary<string, Object>> kv) || kv.Key == null)
                return;

            kv.Key.Unload(false);
            kv = new KeyValuePair<AssetBundle, Dictionary<string, Object>>(null, resStock[packageName].Value);
        }


        public void UnloadAsset(string packageName, string resourceName)
        {
            if (!resStock.TryGetValue(packageName, out KeyValuePair<AssetBundle, Dictionary<string, Object>> kv))
                return;

            if (!kv.Value.TryGetValue(resourceName, out Object asset))
                return;

            Resources.UnloadAsset(asset);
            kv.Value.Remove(resourceName);
        }

        public void UnloadAsset(string resourcePath)
        {
            if (!resStock[resTag].Value.TryGetValue(resourcePath, out Object asset))
                return;

            Resources.UnloadAsset(asset);
            resStock[resTag].Value.Remove(resourcePath);
        }

        public void Clear()
        {
            mainPackage = null;
            maniFest = null;
            resStock.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
            Resources.UnloadUnusedAssets();
        }
    }
}