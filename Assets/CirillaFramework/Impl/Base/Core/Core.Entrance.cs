
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cirilla
{
    public sealed partial class Core : MonoBehaviour
    {
        private const string rootName = "CirillaRoot";
        private static GameObject rootGo;
        private static IContainer containerIns;
        private static Core Runtime;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if((rootGo = GameObject.Find(rootName)) == null)
            {
                rootGo = new GameObject(rootName);
                rootGo.AddComponent<EventSystem>();
                rootGo.AddComponent<StandaloneInputModule>();
                Runtime = rootGo.AddComponent<Core>();
                containerIns = IocContainer.instance;
                Runtime.RegisterModule();
                return;
            }

            Runtime = rootGo.GetComponent<Core>();
        }

        private void Awake() => DontDestroyOnLoad(rootGo);

        private void Start() => ProcessesInit();

        private void RegisterModule()
        {
            containerIns.Register<IMVCModule, MVCModule>();
            containerIns.Register<IObserverModule, ObserverModule>();
            containerIns.Register<IResModule, ResModule>();
            containerIns.Register<INetModule, NetModule>();
            containerIns.Register<ICSVModule, CSVModule>();
            containerIns.Register<IGoPoolModule, GoPoolModule>();
        }
    }
}