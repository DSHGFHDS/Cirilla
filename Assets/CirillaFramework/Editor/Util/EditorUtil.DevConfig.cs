
namespace Cirilla.CEditor
{
    public partial class EditorUtil
    {
        private const string DevConfigAssetPath = "DevConfig";

        public static RuntimePlatform platform
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                RuntimePlatform runtimePlatform = devConfig.runtimePlatform;
                UnLoadAsset(devConfig);
                return runtimePlatform;
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.runtimePlatform = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }

        public static string devPath
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string path = devConfig.projectPath;
                UnLoadAsset(devConfig);
                return path;
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.projectPath = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }

        public static string abRoot
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string root = devConfig.rootName;
                UnLoadAsset(devConfig);
                return root;
            }
        }

        public static string mVCFolder
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string folder = devConfig.mVCFolder;
                UnLoadAsset(devConfig);
                return folder != string.Empty ? folder : (mVCFolder = "MVC");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.mVCFolder = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }
        public static string controllerFolder
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string folder = devConfig.controllerFolder;
                UnLoadAsset(devConfig);
                return folder != string.Empty ? folder : (controllerFolder = "Controller");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.controllerFolder = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }
        public static string modelFolder
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string folder = devConfig.modelFolder;
                UnLoadAsset(devConfig);
                return folder != string.Empty ? folder : (modelFolder = "Model");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.modelFolder = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }

        public static string viewFolder
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string folder = devConfig.viewFolder;
                UnLoadAsset(devConfig);
                return folder != string.Empty ? folder : (viewFolder = "View");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.viewFolder = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }

        public static string abExtension
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string ext = devConfig.assetBundleExtension;
                UnLoadAsset(devConfig);
                return ext != string.Empty ? ext.ToLower() : (abExtension = ".pk");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.assetBundleExtension = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }

        public static string resourceFolder
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string folder = devConfig.resourcesFolder;
                UnLoadAsset(devConfig);
                return folder != string.Empty ? folder : (resourceFolder = "RawResources");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.resourcesFolder = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }

        public static string preLoadExt
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string ext = devConfig.preLoadExt;
                UnLoadAsset(devConfig);
                return ext != string.Empty ? ext.ToLower() : (preLoadExt = "_public");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.preLoadExt = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }

        public static string customLoadExt
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string ext = devConfig.customLoadExt;
                UnLoadAsset(devConfig);
                return ext != string.Empty ? ext.ToLower() : (customLoadExt = "_custom");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.customLoadExt = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }
    }
}
