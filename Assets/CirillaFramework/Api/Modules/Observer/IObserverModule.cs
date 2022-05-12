
using System;

namespace Cirilla
{
    public interface IObserverModule
    {
        void Add<T>(T type, Action<object[]> callBack) where T : struct;
        void Remove<T>(T type, Action<object[]> callBack) where T : struct;
        void Dispatch<T>(T type, params object[] args) where T : struct;

        void Clear();
    }
}