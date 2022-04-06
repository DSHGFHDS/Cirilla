using System;
using System.Collections.Generic;

namespace Cirilla
{
    [Serializable]
    public class DataPanelKV
    {
        public List<SerializableData> dataList;

        public bool init;
        public bool foldout;
        public string key;

        public DataPanelKV()
        {
            dataList = new List<SerializableData>() { new SerializableData(DataType.Int) };
            init = false;
            foldout = true;
            key = Guid.NewGuid().ToString();
        }
    }
}
