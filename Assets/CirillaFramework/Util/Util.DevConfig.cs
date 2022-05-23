using UnityEngine;

namespace Cirilla
{
    public partial class Util
    {
        private const string DevConfigAssetPath = "DevConfig";

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

        public static string rawResourceFolder
        {
            get
            {
                DevConfig devConfig = Resources.Load<DevConfig>(DevConfigAssetPath);
                string folder = devConfig.rawResourcesFolder;
                Resources.UnloadAsset(devConfig);
                return folder;
            }
        }
        public static string buildResourcesFolder
        {
            get
            {
                DevConfig devConfig = Resources.Load<DevConfig>(DevConfigAssetPath);
                string path = devConfig.buildResourcesFolder;
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

        public static bool lazyLoad
        {
            get
            {
                DevConfig devConfig = Resources.Load<DevConfig>(DevConfigAssetPath);
                bool value = devConfig.lazyLoad;
                Resources.UnloadAsset(devConfig);
                return value;
            }
        }
    }
}
