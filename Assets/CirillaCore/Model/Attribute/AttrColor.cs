using System;

namespace Cirilla
{
    public sealed class AttrColor : Attribute
    {
        public string color { get; private set; }
        public AttrColor(string color)
        {
            this.color = color;
        }
    }
}
