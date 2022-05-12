
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
            containerIns.Register<IObserverModule, ObserverModule>();
            containerIns.Register<IResModule, ResModule>();
            containerIns.Register<INetModule, NetModule>();
            containerIns.Register<ICSVModule, CSVModule>();
            containerIns.Register<IGoPoolModule, GoPoolModule>();
        }
    }
}