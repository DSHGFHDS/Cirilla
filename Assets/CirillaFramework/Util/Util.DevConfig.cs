using UnityEngine;

namespace Cirilla
{
    public partial class Util
    {
        private const string DevConfigAssetPath = "DevConfig";

        public static RuntimePlatform platform
        {
            get
            {
                DevConfig devConfig = Resources.Load<DevConfig>(DevConfigAssetPath);
                RuntimePlatform runtimePlatform = devConfig.runtimePlatform;
                Resources.UnloadAsset(devConfig);
                return runtimePlatform;
            }
        }

        public static string devPath
        {
            get
            {
                DevConfig devConfig = Resources.Load<DevConfig>(DevConfigAssetPath);
                string path = devConfig.projectPath;
                Resources.UnloadAsset(devConfig);
                return path;
            }
        }

        public static string abRoot
        {
            get
            {
                DevConfig devConfig = Resources.Load<DevConfig>(DevConfigAssetPath);
                string root = devConfig.rootName;
                Resources.UnloadAsset(devConfig);
                return root;
            }
        }

        public static string abExtension
        {
            get
            {
                DevConfig devConfig = Resources.Load<DevConfig>(DevConfigAssetPath);
                string ext = devConfig.assetBundleExtension;
                Resources.UnloadAsset(devConfig);
                return ext.ToLower();
            }
        }
        public static string resourceFolder
        {
            get
            {
                DevConfig devConfig = Resources.Load<DevConfig>(DevConfigAssetPath);
                string path = devConfig.resourcesFolder;
                Resources.UnloadAsset(devConfig);
                return path;
            }
        }
        public static string preLoadExt
        {
            get
            {
                DevConfig devConfig = Resources.Load<DevConfig>(DevConfigAssetPath);
                string ext = devConfig.preLoadExt;
                Resources.UnloadAsset(devConfig);
                return ext.ToLower();
            }
        }

        public static string customLoadExt
        {
            get
            {
                DevConfig devConfig = Resources.Load<DevConfig>(DevConfigAssetPath);
                string ext = devConfig.customLoadExt;
                Resources.UnloadAsset(devConfig);
                return ext.ToLower();
            }
        }
    }
}
