
using System;
using System.Collections;
using UnityEngine;

namespace Cirilla
{
    public interface IProcess
    {
        void InjectCallback(Action<int, object[]> changeCallback, Func<IEnumerator, Coroutine> startCoroutinCallback, Action<Coroutine> stopCoroutinCallback, Action stopAllCoroutinsCallback);
        void Init();
        void OnEnter(params object[] args);
        void OnInputUpdate();
        void OnPhysicUpdate();
        void OnLogicUpdatePre();
        void OnLogicUpdatePost();
        void OnExit();
        void Change(int processIndex, params object[] args);
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
        void StopAllCoroutines();
    }
}
