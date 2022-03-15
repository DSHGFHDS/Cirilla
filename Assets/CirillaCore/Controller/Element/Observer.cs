
using System.Collections.Generic;

namespace Cirilla
{
    public class Observer<T> : IObserver<T> where T : struct
    {
        private Dictionary<T, observerDelegate> stock;
        public Observer()
        {
            stock = new Dictionary<T, observerDelegate>();
        }

        public void Add(T type, observerDelegate callBack)
        {
            if (!stock.ContainsKey(type))
            {
                stock.Add(type, callBack);
                return;
            }

            foreach (observerDelegate method in stock[type].GetInvocationList())
            {
                if (method != callBack)
                    continue;

                CiriDebugger.LogWarning("Method does exist：" + method.Method.Name);
                return;
            }

            stock[type] += callBack;
        }

        public void Remove(T type, observerDelegate callBack)
        {
            if (callBack == null)
                return;

            if (!stock.ContainsKey(type))
            {
                CiriDebugger.LogWarning("Method doesn't exist：" + callBack.ToString());
                return;
            }

            if ((stock[type] -= callBack) != null)
                return;

            stock.Remove(type);
        }

        public void Dispatch(T type, params object[] args)
        {
            if (!stock.ContainsKey(type))
                return;

            if (stock[type] == null)
            {
                stock.Remove(type);
                return;
            }

            stock[type](args);
        }
        
        public void Clear()
        {
            stock.Clear();
        }
    }
}