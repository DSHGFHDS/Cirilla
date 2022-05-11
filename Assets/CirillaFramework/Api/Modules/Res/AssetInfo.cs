using UnityEngine;

namespace Cirilla
{
    public class AssetInfo
    {
        public Object obj;
        public string bundleName;

        public AssetInfo(Object obj, string bundleName)
        {
            this.obj = obj;
            this.bundleName = bundleName;
        }
    }
}
