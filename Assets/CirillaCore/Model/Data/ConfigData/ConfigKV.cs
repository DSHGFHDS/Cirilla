using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirilla
{
    [Serializable]
    public class ConfigKV
    {
        [SerializeField]
        private int intV;
        [SerializeField]
        private long longV;
        [SerializeField]
        private float floatV;
        [SerializeField]
        private double doubleV;
        [SerializeField]
        private bool boolV;
        [SerializeField]
        private string stringV = "";
        [SerializeField]
        private Object objectV;
        [SerializeField]
        private Color colorV;
        [SerializeField]
        private Vector2 vector2V;
        [SerializeField]
        private Vector3 vector3V;

        public bool init = false;
        public bool foldout = true;
        public ConfigDataType type;
        public string key;

        public object GetValue() {
            switch (type)
            {
                case ConfigDataType.Int:
                    return (object)intV;
                case ConfigDataType.Long:
                    return (object)longV;
                case ConfigDataType.Float:
                    return (object)floatV;
                case ConfigDataType.Double:
                    return (object)doubleV;
                case ConfigDataType.Bool:
                    return (object)boolV;
                case ConfigDataType.String:
                    return (object)stringV;
                case ConfigDataType.Object:
                    return (object)objectV;
                case ConfigDataType.Color:
                    return (object)colorV;
                case ConfigDataType.Vector2:
                    return (object)vector2V;
                case ConfigDataType.Vector3:
                    return (object)vector3V;
            }

            return null;
        }

        public void SetValue(object value)
        {
            switch (type)
            {
                case ConfigDataType.Int:
                    intV = (int)value;
                    break;
                case ConfigDataType.Long:
                    longV = (long)value;
                    break;
                case ConfigDataType.Float:
                    floatV = (float)value;
                    break;
                case ConfigDataType.Double:
                    doubleV = (double)value;
                    break;
                case ConfigDataType.Bool:
                    boolV = (bool)value;
                    break;
                case ConfigDataType.String:
                    stringV = value.ToString();
                    break;
                case ConfigDataType.Object:
                    objectV = (Object)value;
                    break;
                case ConfigDataType.Color:
                    colorV = (Color)value;
                    break;
                case ConfigDataType.Vector2:
                    vector2V = (Vector2)value;
                    break;
                case ConfigDataType.Vector3:
                    vector3V = (Vector3)value;
                    break;
            }
        }
    }
}
