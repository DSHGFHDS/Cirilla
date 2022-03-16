
using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class ConfigData : ScriptableObject
    {
        public List<ConfigKV> kvBuffer = new List<ConfigKV>();
    }
}
