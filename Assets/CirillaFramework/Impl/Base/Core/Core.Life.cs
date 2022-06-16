
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Cirilla
{
    public sealed partial class Core
    {
        public static Action onInputUpdate;
        public static Action onPhysicUpdate;
        public static Action onLogicUpdatePre;
        public static Action onLogicUpdatePost;

        private ConcurrentQueue<MessageInfo> messageQueue;
        private IProcess runningProcess { get; set; }
        private List<IProcess> InitedProcesses;
        private IMVCModule mVCModule;
        private void ProcessesInit()
        {
            InitedProcesses = new List<IProcess>();
            messageQueue = new ConcurrentQueue<MessageInfo>();
            string assemblyName = Util.devPath.Substring("Assets/".Length);
#if UNITY_EDITOR
            string dllPath = Environment.CurrentDirectory.Replace("\\", "/") + $"/Library/ScriptAssemblies/{Util.devPath.Substring("Assets/".Length)}.dll";
            if (!File.Exists(dllPath))
            {
                CiriDebugger.LogWarning(dllPath + "不存在");
                return;
            }
            byte[] dllBytes = File.ReadAllBytes(dllPath);
            
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_WIN
            byte[] dllBytes = containerIns.Resolve<IResModule>().LoadAsset<TextAsset>($"{assemblyName}{Util.preLoadExt}/{assemblyName}.bytes")?.bytes;
#elif UNITY_ANDROID
#elif ENABLE_MICROPHONE
#endif
            if (dllBytes == null)
                return;

            Assembly assembly = Assembly.Load(dllBytes);
            Type type = assembly.GetType(assemblyName+".ProcessType");

            if(type == null)
            {
                CiriDebugger.LogError("ProcessType域名空间不正确");
                return;
            }

            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            if (fieldInfos == null || fieldInfos.Length <= 0)
            {
                CiriDebugger.LogError("缺少可载入流程");
                return;
            }

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                ProcessInfoAttribute attribute = fieldInfos[i].GetCustomAttribute<ProcessInfoAttribute>();
                if (attribute == null)
                    continue;

                containerIns.Register<IProcess>(attribute.type, attribute.type.Name);
            }
            mVCModule = containerIns.Resolve<IMVCModule>();
            Change((Enum)type.GetEnumValues().GetValue(0));
        }

        private void Change(Enum processEnum, params object[] args)
        {
            IProcess process = containerIns.Resolve<IProcess>(processEnum.ToString());

            if (runningProcess == process)
                return;

            if (!InitedProcesses.Contains(process))
            {
                process.InjectCallback(Change);
                process.Init();
                InitedProcesses.Add(process);
                mVCModule.InjectController(containerIns.GetContentInfo<IProcess>(processEnum.ToString()));
            }

            if (runningProcess != null)
                runningProcess.OnExit();

            runningProcess = process;
            runningProcess.OnEnter(args);
        }

        private void Update()
        {
            onInputUpdate?.Invoke();
            runningProcess?.OnInputUpdate();
            onLogicUpdatePre?.Invoke();
            runningProcess?.OnLogicUpdatePre();

            MessageInfo message;
            messageQueue.TryDequeue(out message);
            if (message == null)
                return;

            message.callBack(message.args);
        }

        private void LateUpdate()
        {
            onLogicUpdatePost?.Invoke();
            runningProcess?.OnLogicUpdatePost();
        }

        private void FixedUpdate()
        {
            onPhysicUpdate?.Invoke();
            runningProcess?.OnPhysicUpdate();
        }

        public static void Push(Action<object[]> callback, params object[] args) => Runtime.messageQueue.Enqueue(new MessageInfo(callback, args));

        public static Coroutine StartCoroutine(float time, Action<object[]> callBack, params object[] args) => ((MonoBehaviour)Runtime).StartCoroutine(Runtime.TaskOver(time, callBack, args));
        public static Coroutine StartCoroutine(float time, Action callBack) => ((MonoBehaviour)Runtime).StartCoroutine(Runtime.TaskOver(time, callBack));
        public static new Coroutine StartCoroutine(IEnumerator routine) => ((MonoBehaviour) Runtime).StartCoroutine(routine);

        public static new void StopCoroutine(Coroutine routine) => ((MonoBehaviour)Runtime).StopCoroutine(routine);

        public static new void StopAllCoroutines() => ((MonoBehaviour)Runtime).StopAllCoroutines();

        public static GameObject CirillaGiveBirth(GameObject prefabGo) => GameObject.Instantiate(prefabGo, rootGo.transform);
        public static void AttachToCirilla(GameObject go) => go.transform.SetParent(rootGo.transform);

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
