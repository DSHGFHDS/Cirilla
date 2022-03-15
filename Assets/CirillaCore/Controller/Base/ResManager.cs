using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cirilla
{
    public class ResManager : AMonoSingletonBase<ResManager>
    {
        private Dictionary<string, AssetBundle> abStock;
        private Dictionary<string, Object> assetStock;
        private AssetBundle mainPackage;
        private AssetBundleManifest maniFest;

        private static string pathUrl = Application.streamingAssetsPath + "/";

        private static string mainName =
#if UNITY_IOS
             "IOS";
#elif UNITY_ANDROID
             "Android";
#else
             "PC";
#endif
        private ResManager() {
        }

        protected override void Init()
        {
            abStock = new Dictionary<string, AssetBundle>();
            assetStock = new Dictionary<string, Object>();
        }

        private bool LoadMainPackage()
        {
            if (mainPackage != null)
                return true;

            if ((mainPackage = AssetBundle.LoadFromFile(pathUrl + mainName)) == null)
            {
                CiriDebugger.LogError(pathUrl + mainName + " 无法找到主包,请检查");
                return false;
            }

            if ((maniFest = mainPackage.LoadAsset<AssetBundleManifest>("AssetBundleManifest")) != null)
                return true;

            CiriDebugger.LogError(pathUrl + maniFest + " dose not exist");
            return false;
        }

        private bool LoadAllDependencies(string packageName)
        {
            if (!LoadMainPackage())
                return false;

            string[] Buffers = maniFest.GetAllDependencies(packageName);
            for (int i = 0; i < Buffers.Length; i++)
            {
                if (abStock.ContainsKey(Buffers[i]))
                    continue;

                AssetBundle package = AssetBundle.LoadFromFile(pathUrl + packageName);
                if (package == null)
                {
                    CiriDebugger.LogError(pathUrl + packageName + " 依赖包缺失,请检查");
                    return false;
                }

                abStock.Add(Buffers[i], package);
            }

            return true;
        }

        public T LoadAsset<T>(string packageName, string resourceName) where T : Object
        {
            string assetPath = packageName + "/" + resourceName;
            if (assetStock.TryGetValue(assetPath, out Object asset))
                return asset as T;

            if (!abStock.ContainsKey(packageName))
            {
                CiriDebugger.LogError(pathUrl + packageName + "is not loaded");
                return null;
            }

            T obj = abStock[packageName].LoadAsset<T>(resourceName);

            if (obj == null)
            {
                CiriDebugger.LogError(pathUrl + packageName + "/" + resourceName + " does not exist");
                return null;
            }

            assetStock[assetPath] = obj;

            return obj;
        }

        public void LoadAssetAsync<T>(string packageName, string resourceName, UnityAction<T> callBack) where T : Object
        {
            StartCoroutine(LoadCoroutine(packageName, resourceName, callBack));
        }

        private IEnumerator LoadCoroutine<T>(string packageName, string resourceName, UnityAction<T> callBack) where T : Object
        {
            string assetPath = packageName + "/" + resourceName;
            if (assetStock.TryGetValue(assetPath, out Object asset))
            {
                callBack(asset as T);
                yield break;
            }

            if (!abStock.ContainsKey(packageName))
            {
                CiriDebugger.LogError(pathUrl + packageName + "is not loaded");
                yield break;
            }

            AssetBundleRequest request = abStock[packageName].LoadAssetAsync<T>(resourceName);
            yield return request;

            if (!request.isDone)
            {
                CiriDebugger.LogError(pathUrl + packageName + "/" + resourceName + "does not exist");
                yield break;
            }

            assetStock.Add(assetPath, request.asset);
            callBack(request.asset as T);
        }

        public T LoadAsset<T>(string resourcePath) where T : Object
        {
            if (assetStock.ContainsKey(resourcePath))
                return assetStock[resourcePath] as T;

            T res = Resources.Load<T>(resourcePath);
            if (res == null)
            {
                CiriDebugger.LogError(resourcePath + " does not exist");
                return null;
            }

            assetStock.Add(resourcePath, res);

            return res;
        }

        public void LoadAssetAsync<T>(string resourcePath, UnityAction<T> callBack) where T : Object
        {
            StartCoroutine(LoadCoroutine(resourcePath, callBack));
        }

        private IEnumerator LoadCoroutine<T>(string resourcePath, UnityAction<T> callBack) where T : Object
        {
            if (assetStock.ContainsKey(resourcePath))
            {
                callBack(assetStock[resourcePath] as T);
                yield break;
            }

            ResourceRequest request = Resources.LoadAsync<T>(resourcePath);
            yield return request;

            if (!request.isDone)
            {
                CiriDebugger.LogError(resourcePath + " does not exist");
                yield break;
            }

            callBack(request.asset as T);
        }

        public string[] GetAllAssetNames(string packageName)
        {
            if (!abStock.ContainsKey(packageName))
                return null;

            return abStock[packageName].GetAllAssetNames();
        }

        public void LoadPackage(string packageName)
        {
            if (abStock.ContainsKey(packageName))
                return;

            AssetBundle package = AssetBundle.LoadFromFile(pathUrl + packageName);
            if (package == null)
            {
                CiriDebugger.LogError(pathUrl + packageName + " does not exist");
                return;
            }

            abStock.Add(packageName, package);
            LoadAllDependencies(packageName);
        }

        public void UnloadPackage(string packageName)
        {
            if (!abStock.ContainsKey(packageName))
                return;

            abStock[name].Unload(false);
            abStock.Remove(packageName);
        }

        public void ClearAssets()
        {
            abStock.Clear();
            assetStock.Clear();
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
            mainPackage = null;
            maniFest = null;
        }
    }
}