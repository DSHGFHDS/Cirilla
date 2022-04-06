
using System;

namespace Cirilla
{
    public interface IContainer
    {
        void Register<T1, T2>(string key = "") where T1 : class where T2 : T1;
        void Register<T>(Type type, string key = "");
        void Register<T>(T instance, string key = "");
        void Unregister<T>(string key = "");
        T Resolve<T>(string key = "") where T : class;
        object Resolve(Type type, string key = "");
        void Clear();
    }
}
