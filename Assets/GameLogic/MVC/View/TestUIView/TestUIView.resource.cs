using Cirilla;
using UnityEngine;

namespace GameLogic
{
    public partial class TestUIView : IView
    {
        private const string bindedPath = "TestView/TestUIView.prefab";

        [DependencyAttribute] IResModule resModule;

        private GameObject viewPrefab;
        private GameObject viewGameObjcet;

        private GameObject newB;
        public void Init()
        {
            viewPrefab = resModule.LoadAsset<GameObject>(bindedPath);
            if(viewPrefab == null)
            {
                CiriDebugger.LogError("TestUIView加载失效");
                return;
            }
            viewGameObjcet = Core.CirillaGiveBirth(viewPrefab);
            ViewEntity viewEntity = viewGameObjcet.GetComponent<ViewEntity>();
            newB = viewEntity.GetGo("newB");
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
