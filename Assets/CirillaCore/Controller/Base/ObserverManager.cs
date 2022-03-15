
namespace Cirilla
{
    public class ObserverManager<T> : ASingletonBase<ObserverManager<T>>, IObserver<T> where T : struct
    {
        private Observer<T> observer;
        private ObserverManager() {
            observer = new Observer<T>();
        }

        public void Add(T type, observerDelegate callBack)
        {
            observer.Add(type, callBack);
        }

        public virtual void Clear()
        {
            CiriDebugger.LogError("不建议清空全局订阅器");
            observer.Clear();
        }

        public void Dispatch(T type, params object[] args)
        {
            observer.Dispatch(type, args);
        }

        public void Remove(T type, observerDelegate callBack)
        {
            observer.Remove(type, callBack);
        }
    }
}
