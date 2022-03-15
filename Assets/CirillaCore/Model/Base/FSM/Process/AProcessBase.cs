
using UnityEngine;

namespace Cirilla
{
    public abstract class AProcessBase : MonoBehaviour, IState<AProcessBase>
    {
        private ObserverManager<FSMEvent> observer;
        protected void Awake()
        {
            observer = ObserverManager<FSMEvent>.instance;
        }

        protected void OnEnable(){
        }

        protected void Start(){
        }

        protected void OnDestroy(){
        }

        protected void OnApplicationQuit(){
        }

        protected void FixedUpdate()
        {
            OnPhysicUpdate();
        }

        protected void Update()
        {
            OnInputUpdate();
            OnLogicUpdatePre();
        }

        protected void LateUpdate(){
            OnLogicUpdatePost();
        }

        public void Change<T>(params object[] args) where T : AProcessBase
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
