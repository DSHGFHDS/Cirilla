using Cirilla;
using UnityEngine;

namespace GameLogic
{
    public partial class UITestView : IView
    {
        private const string bindedPath = "AutoLoadWhenUsing/UITestView/UITestView.prefab";

        [DependencyAttribute] IResModule resModule;

        private GameObject viewPrefab;
        private GameObject viewGameObjcet;

        private GameObject helloWord;
        private GameObject heyPicture;
        private GameObject slot1;
        private GameObject slot2;
        private GameObject slot3;
        private GameObject slot4;
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
            helloWord = viewEntity.GetGo("helloWord");
            heyPicture = viewEntity.GetGo("heyPicture");
            slot1 = viewEntity.GetGo("slot1");
            slot2 = viewEntity.GetGo("slot2");
            slot3 = viewEntity.GetGo("slot3");
            slot4 = viewEntity.GetGo("slot4");
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
