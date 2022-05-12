
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Cirilla
{
    public sealed partial class CirillaCore
    {
        private ConcurrentQueue<MessageInfo> messageQueue;
        private IProcess runningProcess { get; set; }
        private List<IProcess> InitedProcesses;
        private void ProcessesInit()
        {
            InitedProcesses = new List<IProcess>();
            messageQueue = new ConcurrentQueue<MessageInfo>();
#if UNITY_EDITOR
            string dllPath = Environment.CurrentDirectory.Replace("\\", "/") + $"/Library/ScriptAssemblies/{Util.devPath.Substring("Assets/".Length)}.dll";
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_WIN
            string dllPath = Application.streamingAssetsPath + $"/{Util.devPath.Substring("Assets/".Length)}.dll";
#elif UNITY_ANDROID
#elif ENABLE_MICROPHONE
#endif
            if (!File.Exists(dllPath))
            {
                CiriDebugger.LogWarning(dllPath + "doesnt exist");
                return;
            }

            byte[] dllBytes = File.ReadAllBytes(dllPath);
            Assembly assembly = Assembly.Load(dllBytes);

            Type type = null;
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].Name != "ProcessType")
                    continue;

                type = types[i];
                break;
            }

            FieldInfo[] fieldInfos = type?.GetFields(BindingFlags.Static | BindingFlags.Public);
            if (fieldInfos == null || fieldInfos.Length <= 0)
            {
                CiriDebugger.LogError("No processes");
                return;
            }

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                ProcessInfoAttribute attribute = fieldInfos[i].GetCustomAttribute<ProcessInfoAttribute>();
                if (attribute == null)
                    continue;

                containerIns.Register<IProcess>(attribute.type, attribute.type.Name);
            }

            Change((Enum)type.GetEnumValues().GetValue(0));
        }

        private void Change(Enum processEnum, params object[] args)
        {
            IProcess process = containerIns.Resolve<IProcess>(processEnum.ToString());

            if (runningProcess == process)
                return;

            if (!InitedProcesses.Contains(process))
            {
                process.InjectCallback(Change, base.StartCoroutine, base.StopCoroutine, base.StopAllCoroutines);
                process.Init();
                InitedProcesses.Add(process);
            }

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

        private void LateUpdate() => runningProcess?.OnLogicUpdatePost();
        private void FixedUpdate() => runningProcess?.OnPhysicUpdate();

        public static void Push(Action<object[]> callback, params object[] args) => Runtime.messageQueue.Enqueue(new MessageInfo(callback, args));

        public static Coroutine StartCoroutine(float time, Action<object[]> callBack, params object[] args) => ((MonoBehaviour)Runtime).StartCoroutine(Runtime.TaskOver(time, callBack, args));
        public static Coroutine StartCoroutine(float time, Action callBack) => ((MonoBehaviour)Runtime).StartCoroutine(Runtime.TaskOver(time, callBack));
        public static new Coroutine StartCoroutine(IEnumerator routine) => ((MonoBehaviour) Runtime).StartCoroutine(routine);

        public static new void StopCoroutine(Coroutine routine) => ((MonoBehaviour)Runtime).StopCoroutine(routine);

        public static new void StopAllCoroutines() => ((MonoBehaviour)Runtime).StopAllCoroutines();

        private IEnumerator TaskOver(float time, Action<object[]> callBack, params object[] args)
        {
            yield return new WaitForSeconds(time);
            callBack(args);
        }

        private IEnumerator TaskOver(float time, Action callBack)
        {
            yield return new WaitForSeconds(time);
            callBack();
        }
    }
}
