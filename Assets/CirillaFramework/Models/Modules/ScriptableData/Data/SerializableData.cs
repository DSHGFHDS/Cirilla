
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirilla
{
    [Serializable]
    public class SerializableData
    {
        public static string instanceInfoDivider = "a4w5d4aw&*%^*7gowad4";
        public DataType type 
        {
            get { return dataType; }
            set 
            {
                if (dataType == value)
                    return;

                dataType = value; 
                SetDefault(); 
            } 
        }

        [SerializeField]
        private DataType dataType;
        [SerializeField]
        private int intValue;
        [SerializeField]
        private long longValue;
        [SerializeField]
        private float floatValue;
        [SerializeField]
        private double doubleValue;
        [SerializeField]
        private bool boolValue;
        [SerializeField]
        private string stringValue;
        [SerializeField]
        private Object ObjectValue;
        [SerializeField]
        private string instanceInfo;
        [SerializeField]
        private Color colorValue;
        [SerializeField]
        private Vector2 vecotor2Value;
        [SerializeField]
        private Vector3 vecotor3Value;

        public SerializableData(DataType dataType)
        {
            this.dataType = dataType;
            SetDefault();
        }

        public object GetValue()
        {
            switch (dataType)
            {
                case DataType.Int:
                    return intValue;
                case DataType.Long:
                    return longValue;
                case DataType.Float:
                    return floatValue;
                case DataType.Double:
                    return doubleValue;
                case DataType.Bool:
                    return boolValue;
                case DataType.String:
                    return stringValue;
                case DataType.Object:
                    if (ObjectValue == null && !string.IsNullOrEmpty(instanceInfo))
                    {
                        string[] buffer = instanceInfo.Split(new[] { instanceInfoDivider }, StringSplitOptions.None);
                        int instanceID = int.Parse(buffer[1]);
                        
                        Object[] objs = Resources.FindObjectsOfTypeAll(typeof(GameObject));
                        for (int i = 0; i < objs.Length; i++)
                            if (objs[i].name == buffer[0] && objs[i].GetInstanceID() == instanceID)
                                return ObjectValue = objs[i];
                    }
                    return ObjectValue;
                case DataType.Color:
                    return colorValue;
                case DataType.Vector2:
                    return vecotor2Value;
                case DataType.Vector3:
                    return vecotor3Value;
                default: return null;
            }
        }

        public void SetValue(object value, string instanceInfo = null)
        {
            switch (dataType)
            {
                case DataType.Int:
                    intValue = (int)value;
                    break;
                case DataType.Long:
                    longValue = (long)value;
                    break;
                case DataType.Float:
                    floatValue = (float)value;
                    break;
                case DataType.Double:
                    doubleValue = (double)value;
                    break;
                case DataType.Bool:
                    boolValue = (bool)value;
                    break;
                case DataType.String:
                    stringValue = value.ToString();
                    break;
                case DataType.Object:
                    this.instanceInfo = instanceInfo;
                    ObjectValue = (Object)value;
                    break;
                case DataType.Color:
                    colorValue = (Color)value;
                    break;
                case DataType.Vector2:
                    vecotor2Value = (Vector2)value;
                    break;
                case DataType.Vector3:
                    vecotor3Value = (Vector3)value;
                    break;
            }
        }

        private void SetDefault()
        {
            intValue = 0;
            longValue = 0L;
            floatValue = 0f;
            doubleValue = 0d;
            boolValue = false;
            stringValue = "";
            ObjectValue = null;
            instanceInfo = "";
            colorValue = Color.white;
            vecotor2Value = Vector2.zero;
            vecotor3Value = Vector3.zero;
        }
    }
}
