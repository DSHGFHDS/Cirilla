using System;

namespace Cirilla
{
    public class TypeInfo
    {
        public Type type { get; private set; }
        public bool IsInjected { get; set; }
        public object instance
        {
            get { return ins ?? (ins = Activator.CreateInstance(type)); }
        }
        private object ins { get; set; }
        public TypeInfo(Type type, object ins)
        {
            this.type = type;
            this.ins = ins;
        }

        
    }
}
