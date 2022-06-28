using System;

namespace Cirilla
{
    public class ASingletonBase<T>
    {
        public static T instance { get; private set; }

        static ASingletonBase() => instance = (T)Activator.CreateInstance(typeof(T), true);
    }
}