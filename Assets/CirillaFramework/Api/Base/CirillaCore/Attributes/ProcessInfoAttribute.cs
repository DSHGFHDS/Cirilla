
using System;

namespace Cirilla
{
    public class ProcessInfoAttribute : Attribute
    {
        public Type type;
        public bool foldout;

        public ProcessInfoAttribute(Type type, bool foldout)
        {
            this.type = type;
            this.foldout = foldout;
        }
    }
}
