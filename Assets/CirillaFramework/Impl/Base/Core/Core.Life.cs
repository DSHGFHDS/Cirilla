
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace Cirilla
{
    public sealed partial class Core
    {
        public static Action onInputUpdate;
        public static Action onPhysicUpdate;
        public static Action onLogicUpdatePre;
        public static Action onLogicUpdatePost;

        public static Action loadHotBuffer;
        public static Action loadProcess;

        private ConcurrentQueue<MessageInfo> messageQueue;
        private IProcess runningProcess { get; set; }
        private List<IProcess> InitedProcesses;
        private IMVCModule mVCModule;

        private int waitForSetUp;
        private string streamingResourcePath;
        private string persistentResourcePath;

        private void ProcessesInit()
        {
            streamingResourcePath = Application.streamingAssetsPath + "/" + Util.buildResourcesFolder;
            persistentResourcePath = Application.persistentDataPath + "/" + Util.buildResourcesFolder;
            InitedProcesses = new List<IProcess>();
            messageQueue = new ConcurrentQueue<MessageInfo>();
            if (string.IsNullOrEmpty(Util.devPath))
            {
                CiriDebugger.LogWarning("缺少项目");
                return;
            }

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            if(Util.lazyLoad)
            {
                LoadProcesses();
                return;
            }
            loadProcess = LoadProcesses;
            MatchStreamingAssetToPersistent();
#else
            LoadProcesses();
#endif
        }

        private void MatchStreamingAssetToPersistent()
        {
            StartCoroutine(WWWDownLoad(streamingResourcePath + "/" + Util.matchFile, (bytes) =>
            {
                if (bytes == null)
                {
                    CiriDebugger.LogError("加载失败:缺少版本文件");
                    return;
                }
                Stream stream = new MemoryStream(bytes);
                StreamReader streamReader = new StreamReader(stream);
                string[] lines = streamReader.ReadToEnd().Split('\n');
                streamReader.Close();
                stream.Close();
                SetUpBaseRes(lines, bytes);
                Action check = null;
                check = () =>
                {
                    if (waitForSetUp > 0)
                        return;

                    GC.Collect();
                    loadHotBuffer?.Invoke();
                    onLogicUpdatePost -= check;
                };
                onLogicUpdatePost += check;
            }));
        }

        private void SetUpBaseRes(string[] lines, byte[] bytes)
        {
            Dictionary<string, ResMatchInfo> baseMatchInfos = new Dictionary<string, ResMatchInfo>();
            for(int i = 1; i < lines.Length; i ++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                string[] info = lines[i].Split('|');
                baseMatchInfos.Add(info[0], new ResMatchInfo(info[0], info[1]));
            }

            string persisMatchFile = persistentResourcePath + "/" + Util.matchFile;
            if (!File.Exists(persisMatchFile))
            {
                Directory.CreateDirectory(persistentResourcePath);
                File.WriteAllBytes($"{persistentResourcePath}/{Util.matchFile}", bytes);
                foreach (ResMatchInfo resMatchInfo in baseMatchInfos.Values)
                {
                    waitForSetUp++;
                    StartCoroutine(WWWDownLoad($"{streamingResourcePath}/{resMatchInfo.file}", (bytes) =>
                    {
                        File.WriteAllBytes($"{persistentResourcePath}/{resMatchInfo.file}", bytes);
                        waitForSetUp --;
                    }));
                }
                return;
            }

            StreamReader streamReader = new StreamReader(persisMatchFile);
            string[] persisInfos = streamReader.ReadToEnd().Split('\n');
            streamReader.Close();

            Dictionary<string, ResMatchInfo> persisMatchInfos = new Dictionary<string, ResMatchInfo>();
            for (int i = 1; i < persisInfos.Length; i++)
            {
                if (string.IsNullOrEmpty(persisInfos[i]))
                    continue;
                string[] info = persisInfos[i].Split('|');
                persisMatchInfos.Add(info[0], new ResMatchInfo(info[0], info[1]));
            }

            int version = int.Parse(persisInfos[0]);
            int baseVer = int.Parse(lines[0]);

            if (baseVer > version)
            {
                File.WriteAllBytes($"{persistentResourcePath}/{Util.matchFile}", bytes);
                foreach (KeyValuePair<string, ResMatchInfo> kv in baseMatchInfos)
                {
                    waitForSetUp ++;
                    StartCoroutine(WWWDownLoad($"{streamingResourcePath}/{kv.Key}", (bytes) =>
                    {
                        File.WriteAllBytes($"{persistentResourcePath}/{kv.Key}", bytes);
                        waitForSetUp --;
                    }));
                }
                return;
            }

            foreach (KeyValuePair<string, ResMatchInfo> kv in baseMatchInfos)
            {
                if (!File.Exists($"{persistentResourcePath}/{kv.Key}"))
                {
                    waitForSetUp ++;
                    StartCoroutine(WWWDownLoad(streamingResourcePath + "/" + kv.Key, (bytes) =>
                    {
                        File.WriteAllBytes($"{persistentResourcePath}/{kv.Key}", bytes);
                        waitForSetUp --;
                    }));
                    continue;
                }
            }
        }

        private void LoadProcesses()
        {
            loadProcess = null;
            string assemblyName = Util.devPath.Substring("Assets/".Length);
#if UNITY_EDITOR
            string dllPath = Environment.CurrentDirectory.Replace("\\", "/") + $"/Library/ScriptAssemblies/{Util.devPath.Substring("Assets/".Length)}.dll";
            if (!File.Exists(dllPath))
            {
                CiriDebugger.LogWarning(dllPath + "不存在");
                return;
            }
            byte[] dllBytes = File.ReadAllBytes(dllPath);

#else
            byte[] dllBytes = containerIns.Resolve<IResModule>().LoadAsset<TextAsset>($"{assemblyName}{Util.preLoadExt}/{assemblyName}.bytes")?.bytes;
#endif
            if (dllBytes == null)
                return;

            Assembly assembly = Assembly.Load(dllBytes);
            Type type = assembly.GetType(assemblyName + ".ProcessType");

            if (type == null)
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

        private IEnumerator WWWDownLoad(string url, Action<byte[]> callBack)
        {
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                callBack?.Invoke(null);
                yield break;
            }

            byte[] bytes = unityWebRequest.downloadHandler.data;

            callBack?.Invoke(bytes.Length > 0 ? bytes : null);
        }
    }
}
