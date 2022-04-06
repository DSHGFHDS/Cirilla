
using UnityEngine;

namespace Cirilla
{
    public interface IGoPool
    {
        GameObject Acquire(GameObject prefab);
        void Recycle(GameObject go);
        void Load(GameObject prefab, int capacity);
        void Unload(GameObject prefab);
        void Clear();
    }
}
