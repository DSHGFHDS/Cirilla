using System;

namespace Cirilla
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DependencyAttribute : Attribute
    {
        public string key { get; private set; }
        public DependencyAttribute(string key = ""){
            this.key = key;
        }
    }
}
