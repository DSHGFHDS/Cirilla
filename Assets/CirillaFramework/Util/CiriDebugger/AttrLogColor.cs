using System;

namespace Cirilla
{
    public sealed class LogColorAttribute : Attribute
    {
        public string color { get; private set; }
        public LogColorAttribute(string color)
        {
            this.color = color;
        }
    }
}
