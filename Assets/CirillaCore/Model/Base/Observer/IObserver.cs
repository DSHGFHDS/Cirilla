
namespace Cirilla
{
    public delegate void observerDelegate(params object[] args);
    public interface IObserver<T> where T : struct
    {
        void Add(T type, observerDelegate callBack);
        void Remove(T type, observerDelegate callBack);
        void Dispatch(T type, params object[] args);
        void Clear();
    }
}