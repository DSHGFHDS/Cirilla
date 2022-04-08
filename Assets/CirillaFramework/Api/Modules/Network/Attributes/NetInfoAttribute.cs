
using System;

namespace Cirilla
{
    public class NetInfoAttribute : Attribute
    {
        public Type type;
        public string url;
        public bool foldout;

        public NetInfoAttribute(Type type, string url, bool foldout)
        {
            this.type = type;
            this.url = url;
            this.foldout = foldout;
        }
    }
}
