using UnityEngine;
using UnityEditor;

namespace Cirilla
{
    public partial class Util
    {
        public static void AddTag(string tag, GameObject obj)
        {
            if (isHasTag(tag))
                return;

                SerializedObject tagManager = new SerializedObject(obj);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name != "m_TagString")
                        continue;

                        Debug.Log(it.stringValue);
                        it.stringValue = tag;
                        tagManager.ApplyModifiedProperties();
                }
        }
        public static bool isHasTag(string tag)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.tags[i].Equals(tag))
                    return true;
            }
            return false;
        }
    
    }
}
