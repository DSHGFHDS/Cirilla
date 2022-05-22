using Cirilla;
using UnityEngine;

namespace GameLogic
{
    public partial class UIView : IView
    {
        private const string bindedPath = "TEST_Public/FFFFFFAD/UIView.prefab";

        [DependencyAttribute] IResModule resModule;

        private GameObject viewPrefab;
        private GameObject viewGameObjcet;

        public void Init()
        {
            viewPrefab = resModule.LoadAsset<GameObject>(bindedPath);
            if(viewPrefab == null)
            {
                CiriDebugger.LogError("UIView加载失效");
                return;
            }
            viewGameObjcet = GameObject.Instantiate<GameObject>(viewPrefab);
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
