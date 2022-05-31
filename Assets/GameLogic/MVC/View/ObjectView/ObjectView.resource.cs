using Cirilla;
using UnityEngine;

namespace GameLogic
{
    public partial class ObjectView : IView
    {
        private const string bindedPath = "ObjectView.prefab";

        [DependencyAttribute] IResModule resModule;

        private GameObject viewPrefab;
        private GameObject viewGameObjcet;

        public GameObject gameObject;
        public void Init()
        {
            viewPrefab = resModule.LoadAsset<GameObject>(bindedPath);
            if(viewPrefab == null)
            {
                CiriDebugger.LogError("ObjectView加载失效");
                return;
            }
            viewGameObjcet = Core.CirillaGiveBirth(viewPrefab);
            ViewEntity viewEntity = viewGameObjcet.GetComponent<ViewEntity>();
            gameObject = viewEntity.GetGo("gameObject");
            VeiwInit();
        }
        public void Dispose()
        {
            if(viewPrefab == null)
                return;
            GameObject.Destroy(viewGameObjcet);
            resModule.UnLoadAsset(viewPrefab);
            VeiwDispose();
        }
    }
}
