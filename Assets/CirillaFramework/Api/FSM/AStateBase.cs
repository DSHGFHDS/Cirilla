
using System;

namespace Cirilla
{
    public abstract class AStateBase : IState
    {
        private Action<Type, object[]> callback;
        public void InjectCallBack(Action<Type, object[]> callback){
            this.callback = callback;
        }

        public void Change<T>(params object[] args) where T : IState{
            callback(typeof(T), args);
        }

        public abstract void Init();
        public abstract void OnEnter(params object[] args);
        public abstract void OnUpdate();
        public abstract void OnExit();
    }
}