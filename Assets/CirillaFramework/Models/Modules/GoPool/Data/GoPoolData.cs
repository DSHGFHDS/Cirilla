using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class GoPoolData
    {
        public Dictionary<GameObject, bool> pool { get; private set; }
        public int capacity { get; private set; }
        public GoPoolData(Dictionary<GameObject, bool> pool, int capacity)
        {
            this.pool = pool;
            this.capacity = capacity;
        }
    }
}
