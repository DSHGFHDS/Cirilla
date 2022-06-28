using UnityEngine;

namespace Cirilla
{
    public class DevConfig : ScriptableObject
    {
        public string projectPath;
        public string mVCFolder = "MVC";
        public string controllerFolder = "Controller";
        public string modelFolder = "Model";
        public string viewFolder = "View";
        public string assetBundleExtension = ".pk";
        public string rawResourcesFolder = "RawResources";
        public string buildResourcesFolder = "BuildResources";
        public string matchFile = "version";
        public string preLoadExt = "_public";
        public string customLoadExt = "_custom";
        public string baseSourceExt = "_base";
        public bool lazyLoad = false;
        public string rootName { get { return "root"; } }
        public int baseVersion;
    }
}
