using System;

namespace Cirilla
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ModelAttribute : Attribute
    {
        public string key { get; private set; }
        public ModelAttribute(string key = "")
        {
            this.key = key;
        }
    }
}
