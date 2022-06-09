using Cirilla;
using UnityEngine;

namespace GameLogic
{
    public partial class WorldView : IView
    {
        private const string bindedPath = "WorldView.prefab";

        [DependencyAttribute] IResModule resModule;

        private GameObject viewPrefab;
        private GameObject viewGameObjcet;

        public GameObject spawnPoint;
        public void Init()
        {
            viewPrefab = resModule.LoadAsset<GameObject>(bindedPath);
            if(viewPrefab == null)
            {
                CiriDebugger.LogError("WorldView加载失效");
                return;
            }
            viewGameObjcet = Core.CirillaGiveBirth(viewPrefab);
            ViewEntity viewEntity = viewGameObjcet.GetComponent<ViewEntity>();
            spawnPoint = viewEntity.GetGo("spawnPoint");
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
