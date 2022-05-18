
using System;

namespace Cirilla
{
    public interface IMVCModule
    {
        T Add<T>(string key = "") where T : class, IMVCBase;
        object Add(Type type, string key = "");
        void Remove<T>(string key = "") where T : class, IMVCBase;
        void Remove(Type type, string key = "");
        void InjectController(ContentInfo contentInfo);
    }
}
