
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
            if((rootGo = GameObject.Find(rootName)) == null)
            {
                rootGo = new GameObject(rootName);
                Runtime = rootGo.AddComponent<CirillaCore>();
                containerIns = IocContainer.instance;
                Runtime.RegisterModule();
                return;
            }

            Runtime = rootGo.GetComponent<CirillaCore>();
        }

        private void Awake() => DontDestroyOnLoad(rootGo);

        private void Start() => ProcessesInit();

        private void RegisterModule()
        {
            containerIns.Register<IObserver, ObserverModule>();
            containerIns.Register<INet, NetModule>();
            containerIns.Register<IRes, ResModule>();
            containerIns.Register<ICSV, CSVModule>();
            containerIns.Register<IScriptableData, ScriptableDataModule>();
            containerIns.Register<IGoPool, GoPoolModule>();
        }
    }
}