using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cirilla
{
    public class ContentInfo
    {
        public Type type { get; private set; }
        public string key { get; private set; }
        public List<FieldInfo> fieldInfos; 
        public bool IsInjected { get; set; }
        public object obj { get { return ins ?? (ins = Activator.CreateInstance(type)); } }
        private object ins { get; set; }
        public ContentInfo(Type type, string key, object ins)
        {
            this.type = type;
            this.key = key;
            this.ins = ins;
            fieldInfos = new List<FieldInfo>();
        }
    }
}
