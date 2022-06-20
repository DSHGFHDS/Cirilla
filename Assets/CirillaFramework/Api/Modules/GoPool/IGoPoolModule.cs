
using UnityEngine;

namespace Cirilla
{
    public interface IGoPoolModule
    {
        GameObject Acquire(GameObject prefab);
        void Recycle(GameObject go);
        void Load(GameObject prefab, int capacity);
        void Unload(GameObject prefab);
        void Shrink();
        void Clear();
    }
}
