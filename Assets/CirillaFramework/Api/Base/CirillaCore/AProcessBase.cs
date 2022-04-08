
using System;
using System.Collections;
using UnityEngine;

namespace Cirilla
{
    public abstract class AProcessBase : IProcess
    {
        private Action<int, object[]> changeCallback;
        private Func<IEnumerator, Coroutine> startCoroutinCallback;
        private Action<Coroutine> stopCoroutinCallback;
        private Action stopAllCoroutinsCallback;
        public void InjectCallback(Action<int, object[]> changeCallback, Func<IEnumerator, Coroutine> startCoroutinCallback, Action<Coroutine> stopCoroutinCallback, Action stopAllCoroutinsCallback)
        {
            this.changeCallback = changeCallback;
            this.startCoroutinCallback = startCoroutinCallback;
            this.stopCoroutinCallback = stopCoroutinCallback;
            this.stopAllCoroutinsCallback = stopAllCoroutinsCallback;
        }

        public void Change(int processIndex, params object[] args){
            changeCallback(processIndex, args);
        }

        public Coroutine StartCoroutine(IEnumerator routine){
            return startCoroutinCallback(routine);
        }

        public void StopCoroutine(Coroutine routine){
            stopCoroutinCallback(routine);
        }

        public void StopAllCoroutines(){
            stopAllCoroutinsCallback();
        }

        public abstract void Init();
        public abstract void OnEnter(params object[] args);
        public abstract void OnInputUpdate();
        public abstract void OnLogicUpdatePost();
        public abstract void OnLogicUpdatePre();
        public abstract void OnPhysicUpdate();
        public abstract void OnExit();

        protected void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

    }
}
