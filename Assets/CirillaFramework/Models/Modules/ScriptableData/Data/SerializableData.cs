
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cirilla
{
    [Serializable]
    public class SerializableData
    {
        public DataType type;
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
        private Color colorValue;
        [SerializeField]
        private Vector2 vecotor2Value;
        [SerializeField]
        private Vector3 vecotor3Value;

        public SerializableData(DataType type)
        {
            this.type = type;
            SetDefault();
        }

        public object GetValue()
        {
            switch (type)
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

        public void SetValue(object value)
        {
            SetDefault();
            if (value is int)
            {
                intValue = (int)value;
                type = DataType.Int;
                return;
            }

            if (value is long)
            {
                longValue = (long)value;
                type = DataType.Long;
                return;
            }

            if (value is float)
            {
                floatValue = (float)value;
                type = DataType.Float;
                return;
            }

            if (value is double)
            {
                doubleValue = (double)value;
                type = DataType.Double;
                return;
            }

            if (value is bool)
            {
                boolValue = (bool)value;
                type = DataType.Bool;
                return;
            }

            if (value is string)
            {
                stringValue = value.ToString();
                type = DataType.String;
                return;
            }

            if (value is Object)
            {
                ObjectValue = (Object)value;
                type = DataType.Object;
                return;
            }

            if (value is Color)
            {
                colorValue = (Color)value;
                type = DataType.Color;
                return;
            }

            if (value is Vector2)
            {
                vecotor2Value = (Vector2)value;
                type = DataType.Vector2;
                return;
            }

            if (value is Vector3)
            {
                vecotor3Value = (Vector3)value;
                type = DataType.Vector3;
                return;
            }

            CiriDebugger.LogWarning("Set an error data");
        }

        private void SetDefault()
        {
            this.intValue = 0;
            this.longValue = 0L;
            this.floatValue = 0f;
            this.doubleValue = 0d;
            this.boolValue = false;
            this.stringValue = "";
            this.ObjectValue = Util.unNullUnityObject;
            this.colorValue = Color.white;
            this.vecotor2Value = Vector2.zero;
            this.vecotor3Value = Vector3.zero;
        }
    }
}
