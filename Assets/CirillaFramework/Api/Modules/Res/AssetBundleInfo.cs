using UnityEngine;

namespace Cirilla
{
    public class AssetBundleInfo
    {
        public AssetBundle assetBundle;
        public int assetLoaded;

        public AssetBundleInfo(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
        }
    }
}
