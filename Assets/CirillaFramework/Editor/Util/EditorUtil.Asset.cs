using UnityEditor;
using UnityEngine;

namespace Cirilla.CEditor
{
    public partial class EditorUtil
    {
        public static void SaveAsset(Object obj)
        {
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static T LoadAsset<T>(string path) where T : Object => AssetDatabase.LoadAssetAtPath<T>(path);
        public static T LoadAssetFromResources<T>(string path) where T : Object => Resources.Load<T>(path);
        public static void UnLoadAsset(Object obj) => Resources.UnloadAsset(obj);
    }
}
