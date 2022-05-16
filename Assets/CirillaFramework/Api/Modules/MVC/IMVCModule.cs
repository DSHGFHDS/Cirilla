
using System;

namespace Cirilla
{
    public interface IMVCModule
    {
        T Add<T>(string key) where T : class, IController;
        object Add(Type type, string key);
        void InjectController(ContentInfo contentInfo);
    }
}
