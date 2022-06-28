
namespace Cirilla.CEditor
{
    public partial class EditorUtil
    {
        private const string DevConfigAssetPath = "DevConfig";

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

        public static string rawResourceFolder
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string folder = devConfig.rawResourcesFolder;
                UnLoadAsset(devConfig);
                return folder != string.Empty ? folder : (rawResourceFolder = "RawResources");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.rawResourcesFolder = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }

        public static string buildResourcesFolder
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string folder = devConfig.buildResourcesFolder;
                UnLoadAsset(devConfig);
                return folder != string.Empty ? folder : (buildResourcesFolder = "BuildResources");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.buildResourcesFolder = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }

        public static string matchFile
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string file = devConfig.matchFile;
                UnLoadAsset(devConfig);
                return file != string.Empty ? file : (buildResourcesFolder = "version");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.matchFile = value;
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

        public static string baseSourceExt
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                string ext = devConfig.baseSourceExt;
                UnLoadAsset(devConfig);
                return ext != string.Empty ? ext.ToLower() : (baseSourceExt = "_base");
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.customLoadExt = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }


        public static bool lazyLoad
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                bool value = devConfig.lazyLoad;
                UnLoadAsset(devConfig);
                return value;
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.lazyLoad = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }

        public static int version
        {
            get
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                int value = devConfig.baseVersion;
                UnLoadAsset(devConfig);
                return value;
            }

            set
            {
                DevConfig devConfig = LoadAssetFromResources<DevConfig>(DevConfigAssetPath);
                devConfig.baseVersion = value;
                SaveAsset(devConfig);
                UnLoadAsset(devConfig);
            }
        }
    }
}
