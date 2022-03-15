using System;
using System.Collections.Generic;

namespace Cirilla
{
    public class FSM : IFSM<AStateBase>, IGameUpdate
    {
        private ObserverManager<FSMEvent> observer;
        private Dictionary<Type, AStateBase> stateStock;

        public Type runningStateType { get; private set; }
        public FSM()
        {
            observer = ObserverManager<FSMEvent>.instance;
            observer.Add(FSMEvent.Change, ChangeCallback);
            stateStock = new Dictionary<Type, AStateBase>();
        }

        ~FSM()
        {
            observer.Remove(FSMEvent.Change, ChangeCallback);
        }

        public void Add<T>() where T : AStateBase
        {
            Type type = typeof(T);

            if (stateStock.ContainsKey(type))
            {
                CiriDebugger.LogWarning("State does exist:" + type.ToString());
                return;
            }

            AStateBase state = (AStateBase)Activator.CreateInstance(type);
            stateStock.Add(type, state);
            state.Init();
        }

        public void Change<T>(params object[] args) where T : AStateBase
        {
            Change(typeof(T), args);
        }

        public void Change(Type type, params object[] args)
        {
            if (runningStateType == type)
                return;

            if (!stateStock.TryGetValue(type, out AStateBase changedState))
            {
                CiriDebugger.LogWarning("State does not exist:" + type.ToString());
                return;
            }

            if (runningStateType != null && stateStock.TryGetValue(runningStateType, out AStateBase runningState))
                runningState.OnExit();

            runningStateType = type;

            changedState.OnEnter(args);
        }

        private void ChangeCallback(params object[] args)
        {
            if (!(args[0] is AStateBase state))
                return;

            if (!stateStock.TryGetValue(state.GetType(), out AStateBase matchState) || matchState != state)
                return;

            if (!(args[1] is Type type))
                return;

            object[] buffer = new object[args.Length - 2];
            for (int i = 2; i < args.Length; i++)
                buffer[i - 2] = args[i];

            Change(type, buffer);
        }
        public void Remove<T>() where T : AStateBase
        {
            Type type = typeof(T);

            if (type == runningStateType)
                runningStateType = null;

            if (!stateStock.ContainsKey(type))
            {
                CiriDebugger.LogWarning("State does not exist:" + type.ToString());
                return;
            }

            stateStock.Remove(type);
            if (runningStateType != type)
                return;

            runningStateType = null;
        }

        public void Clear()
        {
            stateStock.Clear();
            runningStateType = null;
        }

        public void OnInputUpdate()
        {
            if (runningStateType == null)
                return;

            if (!stateStock.TryGetValue(runningStateType, out AStateBase state))
                return;

            state.OnInputUpdate();
        }

        public void OnPhysicUpdate()
        {
            if (runningStateType == null)
                return;

            if (!stateStock.TryGetValue(runningStateType, out AStateBase state))
                return;

            state.OnPhysicUpdate();
        }

        public void OnLogicUpdatePre()
        {
            if (runningStateType == null)
                return;

            if (!stateStock.TryGetValue(runningStateType, out AStateBase state))
                return;

            state.OnLogicUpdatePre();
        }

        public void OnLogicUpdatePost()
        {
            if (runningStateType == null)
                return;

            if (!stateStock.TryGetValue(runningStateType, out AStateBase state))
                return;

            state.OnLogicUpdatePost();
        }
    }
}