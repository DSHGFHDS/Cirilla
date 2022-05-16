using System;

namespace Cirilla
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ViewAttribute : Attribute
    {
        public string key { get; private set; }
        public ViewAttribute(string key = "")
        {
            this.key = key;
        }
    }
}
