using Cirilla;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public partial class UITestView : IView
    {
        private const string bindedPath = "AutoLoadWhenUsing/UITestView/UITestView.prefab";

        [DependencyAttribute] IResModule resModule;

        private GameObject viewPrefab;
        private GameObject viewGameObjcet;

        private Image slot1;
        private Image slot2;
        private Image slot3;
        private Image slot4;
        public void Init()
        {
            viewPrefab = resModule.LoadAsset<GameObject>(bindedPath);
            if(viewPrefab == null)
            {
                CiriDebugger.LogError("UITestView加载失效");
                return;
            }
            viewGameObjcet = Core.CirillaGiveBirth(viewPrefab);
            ViewEntity viewEntity = viewGameObjcet.GetComponent<ViewEntity>();
            slot1 = (Image)viewEntity.GetPointObj("slot1");
            slot2 = (Image)viewEntity.GetPointObj("slot2");
            slot3 = (Image)viewEntity.GetPointObj("slot3");
            slot4 = (Image)viewEntity.GetPointObj("slot4");
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
