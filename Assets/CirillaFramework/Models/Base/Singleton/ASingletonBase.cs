using System;

namespace Cirilla
{
    public class ASingletonBase<T>
    {
        private static T ins;

        static ASingletonBase()
        {
            ins = (T)Activator.CreateInstance(typeof(T), true);
        }

        public static T instance
        {
            get
            {
                return ins;
            }
        }
    }
}