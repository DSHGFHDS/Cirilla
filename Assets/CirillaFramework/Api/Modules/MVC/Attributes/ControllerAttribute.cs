using System;

namespace Cirilla
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ControllerAttribute : Attribute
    {
        public string key { get; private set; }
        public ControllerAttribute(string key = "")
        {
            this.key = key;
        }
    }
}
