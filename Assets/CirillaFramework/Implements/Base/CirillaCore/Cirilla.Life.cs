
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using UnityEngine;

namespace Cirilla
{
    public sealed partial class CirillaCore
    {
        private ConcurrentQueue<MessageInfo> messageQueue;
        private IProcess runningProcess { get; set; }
        private void ProcessesInit()
        {
            messageQueue = new ConcurrentQueue<MessageInfo>();
            Type type = Util.GetTypeFromName("ProcessType", "GameLogic");
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public);

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                ProcessInfoAttribute attribute = fieldInfos[i].GetCustomAttribute<ProcessInfoAttribute>();
                if (attribute == null)
                    continue;

                containerIns.Register<IProcess>(attribute.type, i.ToString());
                IProcess process = containerIns.Resolve<IProcess>(i.ToString());
                process.InjectCallback(Change, base.StartCoroutine, base.StopCoroutine, base.StopAllCoroutines);
                process.Init();
            }

            if(fieldInfos.Length <= 0)
            {
                CiriDebugger.Log("缺少可进入的流程");
                return;
            }    
            Change(0);
        }

        private void Change(int processIndex, params object[] args)
        {
            IProcess process = containerIns.Resolve<IProcess>(processIndex.ToString());

            if (runningProcess == process)
                return;

            if (runningProcess != null)
                runningProcess.OnExit();

            runningProcess = process;
            runningProcess.OnEnter(args);
        }

        private void Update()
        {
            runningProcess?.OnInputUpdate();
            runningProcess?.OnLogicUpdatePre();

            while (messageQueue.Count > 0)
            {
                MessageInfo message;
                messageQueue.TryDequeue(out message);
                if (message == null)
                    continue;

                message.callBack(message.args);
            }
        }

        private void LateUpdate() {
            runningProcess?.OnLogicUpdatePost();
        }

        private void FixedUpdate() {
            runningProcess?.OnPhysicUpdate();
        }

        public static void Push(Action<object[]> callback, params object[] args)
        {
            Runtime.messageQueue.Enqueue(new MessageInfo(callback, args));
        }

        public static new Coroutine StartCoroutine(IEnumerator routine)
        {
            return ((MonoBehaviour)Runtime).StartCoroutine(routine);
        }

        public static new void StopCoroutine(Coroutine routine)
        {
            ((MonoBehaviour)Runtime).StopCoroutine(routine);
        }

        public static new void StopAllCoroutines()
        {
            ((MonoBehaviour)Runtime).StopAllCoroutines();
        }
    }
}
