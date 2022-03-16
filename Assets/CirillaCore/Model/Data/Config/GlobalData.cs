using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class GlobalData : MonoBehaviour
    {
        [HideInInspector]
        public List<ConfigKV> kvBuffer = new List<ConfigKV>();
    }
}
