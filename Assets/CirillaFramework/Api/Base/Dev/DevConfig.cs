using UnityEngine;

namespace Cirilla
{
    public class DevConfig : ScriptableObject
    {
        public RuntimePlatform runtimePlatform;
        public string projectPath;
        public string assetBundleExtension = ".pk";
        public string resourcesFolder = "RawResources";
        public string preLoadExt = "_public";
        public string customLoadExt = "_custom";
        public string rootName { get { return "root"; } }
    }
}
