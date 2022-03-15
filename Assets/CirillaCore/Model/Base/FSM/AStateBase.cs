
namespace Cirilla
{
    public abstract class AStateBase : IState<AStateBase>
    {
        private ObserverManager<FSMEvent> observer;

        private AStateBase() {
            observer = ObserverManager<FSMEvent>.instance;
        }

        public void Change<T>(params object[] args) where T : AStateBase
        {
            object[] buffer = new object[args.Length + 2];
            buffer[0] = this;
            buffer[1] = typeof(T);
            args.CopyTo(buffer, 2);
            observer.Dispatch(FSMEvent.Change, buffer);
        }

        public abstract void Init();
        public abstract void OnEnter(params object[] args);
        public abstract void OnInputUpdate();

        public abstract void OnPhysicUpdate();

        public abstract void OnLogicUpdatePre();
        public abstract void OnLogicUpdatePost();
        public abstract void OnExit();
    }
}