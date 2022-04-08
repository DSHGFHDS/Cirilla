
using System;

namespace Cirilla
{
    public interface IState
    {
        void InjectCallBack(Action<Type, object[]> callback);
        void Init();
        void OnEnter(params object[] args);
        void OnUpdate();
        void OnExit();
        void Change<T>(params object[] args) where T : IState;
    }
}
