
using System;
using System.Collections;
using UnityEngine;

namespace Cirilla
{
    public interface IProcess
    {
        void InjectCallback(Action<Enum, object[]> changeCallback);
        void Init();
        void OnEnter(params object[] args);
        void OnInputUpdate();
        void OnPhysicUpdate();
        void OnLogicUpdatePre();
        void OnLogicUpdatePost();
        void OnExit();
        void Change(Enum processEnum, params object[] args);
        Coroutine StartCoroutine(IEnumerator routine);
        Coroutine StartCoroutine(float time, Action callBack);
        Coroutine StartCoroutine(float time, Action<object[]> callBack, params object[] args);
        void StopCoroutine(Coroutine routine);
        void StopAllCoroutines();
    }
}
