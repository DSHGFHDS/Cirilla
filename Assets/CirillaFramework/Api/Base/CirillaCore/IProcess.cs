
using System;
using System.Collections;
using UnityEngine;

namespace Cirilla
{
    public interface IProcess
    {
        void InjectCallback(Action<Enum, object[]> changeCallback, Func<IEnumerator, Coroutine> startCoroutinCallback, Action<Coroutine> stopCoroutinCallback, Action stopAllCoroutinsCallback);
        void Init();
        void OnEnter(params object[] args);
        void OnInputUpdate();
        void OnPhysicUpdate();
        void OnLogicUpdatePre();
        void OnLogicUpdatePost();
        void OnExit();
        void Change(Enum processEnum, params object[] args);
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
        void StopAllCoroutines();
    }
}
