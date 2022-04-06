using System;
using System.Collections.Generic;

namespace Cirilla
{
    public class FSM : IFSM
    {
        private Dictionary<Type, AStateBase> stateStock;
        private AStateBase runningState { get;  set; }

        public FSM(){
            stateStock = new Dictionary<Type, AStateBase>();
        }

        private void Change(Type type, params object[] args)
        {
            if (!stateStock.TryGetValue(type, out AStateBase state))
            {
                CiriDebugger.LogWarning("State does not exist:" + type.Name);
                return;
            }

            if (runningState == state)
                return;

            if (runningState != null)
                runningState.OnExit();

            runningState = state;

            runningState.OnEnter(args);
        }

        public void Add<T>() where T : AStateBase
        {
            Type type = typeof(T);

            if (stateStock.ContainsKey(type))
            {
                CiriDebugger.LogWarning("State does exist:" + type.Name);
                return;
            }

            AStateBase state = (AStateBase)Activator.CreateInstance(type);
            state.InjectCallBack(new Action<Type, object[]>(Change));
            stateStock.Add(type, state);
            state.Init();
        }

        public void Change<T>(params object[] args) where T : AStateBase
        {
            Change(typeof(T), args);
        }

        public void Remove<T>() where T : AStateBase
        {
            Type type = typeof(T);

            if (!stateStock.TryGetValue(type, out AStateBase state))
            {
                CiriDebugger.LogWarning("State does not exist:" + type.Name);
                return;
            }

            if (state == runningState)
                runningState = null;

            stateStock.Remove(type);
        }

        public void Clear()
        {
            runningState = null;
            stateStock.Clear();
        }

        public void OnUpdate() {
            runningState?.OnUpdate();
        }
    }
}