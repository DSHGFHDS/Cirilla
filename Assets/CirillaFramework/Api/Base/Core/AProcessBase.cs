
using System;
using System.Collections;
using UnityEngine;

namespace Cirilla
{
    public abstract class AProcessBase : IProcess
    {
        private Action<Enum, object[]> changeCallback;

        public void InjectCallback(Action<Enum, object[]> changeCallback) => this.changeCallback = changeCallback;

        public void Change(Enum processEnum, params object[] args) => changeCallback(processEnum, args);

        public Coroutine StartCoroutine(float time, Action callBack) => Core.StartCoroutine(time, callBack);

        public Coroutine StartCoroutine(float time, Action<object[]> callBack, params object[] args) => Core.StartCoroutine(time, callBack, args);

        public Coroutine StartCoroutine(IEnumerator routine) => Core.StartCoroutine(routine);

        public void StopCoroutine(Coroutine routine) => Core.StopCoroutine(routine);

        public void StopAllCoroutines() => StopAllCoroutines();

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
