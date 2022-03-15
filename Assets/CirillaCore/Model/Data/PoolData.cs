using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public partial class GoManager
    {
        private class PoolData
        {
            public Dictionary<GameObject, bool> pool { get; private set; }
            public int capacity { get; private set; }
            public PoolData(Dictionary<GameObject, bool> pool, int capacity)
            {
                this.pool = pool;
                this.capacity = capacity;
            }
        }
    }
}
