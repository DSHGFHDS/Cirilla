using System;
using System.Collections.Generic;

namespace Cirilla
{
    public class ObserverModule : IObserverModule
    {
        private Dictionary<ValueType, Action<object[]>> stock;
        public ObserverModule(){
            stock = new Dictionary<ValueType, Action<object[]>>();
        }

        public void Add<T>(T type, Action<object[]> callBack) where T : struct
        {
            if (!stock.ContainsKey(type))
            {
                stock.Add(type, callBack);
                return;
            }

            foreach (Action<object[]> method in stock[type].GetInvocationList())
            {
                if (method != callBack)
                    continue;

                CiriDebugger.LogWarning("Method does exist：" + method.Method.Name);
                return;
            }

            stock[type] += callBack;
        }

        public void Remove<T>(T type, Action<object[]> callBack) where T : struct
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

        public void Dispatch<T>(T type, params object[] args) where T : struct
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

        [Obsolete("It is dangerous to do this")]
        public void Clear()
        {
            stock.Clear();
        }
    }
}