
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Cirilla
{
    public class ProcessManager : AMonoSingletonBase<ProcessManager>, IProcess
    {
        private ObserverManager<FSMEvent> observer;
        private Dictionary<Type, AProcessBase> processStock;
        public Type runningProType { get; private set; }

        private ProcessManager(){
        }

        protected override void Init()
        {
            observer = ObserverManager<FSMEvent>.instance;
            observer.Add(FSMEvent.Change, ChangeCallback);
            processStock = new Dictionary<Type, AProcessBase>();
            TextAsset textAsset = ConfigManager.instance["Entrance"] as TextAsset;

            if(textAsset == null)
            {
                CiriDebugger.Log("GameLoadProcess is null");
                return;
            }

            Type gameLoadType = null;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes())
                {
                    if (type.Name != textAsset.name)
                        continue;
                    gameLoadType = type;
                    break;
                }
            }

            if(gameLoadType == null)
                return;

            Add(gameLoadType);
            Change(gameLoadType);
        }

        private void Add(Type type)
        {
            if (processStock.ContainsKey(type))
            {
                CiriDebugger.LogWarning("Process does exist:" + type.ToString());
                return;
            }

            AProcessBase pro = goInstance.AddComponent(type) as AProcessBase;
            pro.enabled = false;
            processStock.Add(type, pro);
            pro.Init();
        }

        public void Add<T>() where T : AProcessBase
        {
            Add(typeof(T));
        }

        private void Change(Type type, params object[] args)
        {
            if (runningProType == type)
                return;

            if (!processStock.TryGetValue(type, out AProcessBase changedPro))
            {
                CiriDebugger.LogWarning("Process does not exist:" + type.ToString());
                return;
            }

            if (runningProType != null && processStock.TryGetValue(runningProType, out AProcessBase runningPro))
            {
                runningPro.enabled = false;
                runningPro.OnExit();
                
            }
            runningProType = type;

            if (changedPro == null)
                return;

            changedPro.enabled = true;
            changedPro.OnEnter(args);
        }

        public void Change<T>(params object[] args) where T : AProcessBase
        {
            Change(typeof(T));
        }

        private void ChangeCallback(params object[] args)
        {
            if (!(args[0] is AProcessBase process))
                return;

            if (!processStock.TryGetValue(process.GetType(), out AProcessBase matchPro) || matchPro != process)
                return;

            if (!(args[1] is Type type))
                return;

            object[] buffer = new object[args.Length - 2];
            for (int i = 2; i < args.Length; i++)
                buffer[i - 2] = args[i];

            Change(type, buffer);
        }

        public void Remove<T>() where T : AProcessBase
        {
            CiriDebugger.LogWarning("不建议删除流程");
            Type type = typeof(T);

            if (type == runningProType)
                runningProType = null;

            AProcessBase pro = goInstance.GetComponent(type) as AProcessBase;
            if (pro != null)
                DestroyImmediate(pro);

            if (!processStock.ContainsKey(type))
            {
                CiriDebugger.LogWarning("Process does not exist:" + type.ToString());
                return;
            }

            processStock.Remove(type);
        }

        public virtual void Clear()
        {
            foreach(Type type in processStock.Keys) {
                Component process = goInstance.GetComponent(type);
                if (process == null)
                    continue;

                DestroyImmediate(process);
            }
            processStock.Clear();
            runningProType = null;
        }

        public void ProcessGC()
        {
            GoManager.instance.Clear();
            ConfigManager.instance.Clear();
            ResManager.instance.Clear();
        }

        public void AppQuit()
        {
            ProcessGC();
            Clear();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
