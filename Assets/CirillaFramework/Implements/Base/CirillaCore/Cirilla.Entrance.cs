
using UnityEngine;

namespace Cirilla
{
    public sealed partial class CirillaCore : MonoBehaviour
    {
        private const string rootName = "CirillaRoot";
        private static GameObject rootGo;
        private static IContainer containerIns;
        private static CirillaCore Runtime;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            rootGo = GameObject.Find(rootName) ?? new GameObject(rootName);
            Runtime = rootGo.GetComponent<CirillaCore>() ?? rootGo.AddComponent<CirillaCore>();
            containerIns = IocContainer.instance;
            Runtime.RegisterModule();
            Runtime.ProcessesInit();
        }

        private void Awake()
        {
            DontDestroyOnLoad(rootGo);
        }

        private void RegisterModule()
        {
            containerIns.Register<IObserver, ObserverManager>();
            containerIns.Register<INet, NetManager>();
            containerIns.Register<IRes, ResManager>();
            containerIns.Register<ICSV, CSVManager>();
            containerIns.Register<IScriptableData, ScriptableDataManager>();
            containerIns.Register<IGoPool, GoPoolManager>();
        }
    }
}