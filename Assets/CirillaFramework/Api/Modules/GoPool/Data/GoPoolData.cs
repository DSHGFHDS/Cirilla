
using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class GoPoolData
    {
        public List<GameObject> pool { get; private set; }
        public Queue<GameObject> validPool { get; private set; }
        public GoPoolData(int capacity)
        {
            pool = new List<GameObject>(capacity);
            validPool = new Queue<GameObject>();
        }
    }
}
