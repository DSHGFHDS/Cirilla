
using System.Collections.Generic;
using UnityEngine;

namespace Cirilla
{
    public class DataPanel : ScriptableObject
    {
        public List<DataPanelKV> dataList = new List<DataPanelKV>();
        public bool ContainKey(string key)
        {
            foreach(DataPanelKV kv in dataList)
            {
                if (kv.key != key)
                    continue;

                return true;
            }

            return false;
        }
    }
}
