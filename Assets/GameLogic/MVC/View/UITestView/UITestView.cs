using Cirilla;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public partial class UITestView : IView
    {
        private GameObject earGo;
        #region 初始化与释放
        public void VeiwInit()
        {
            earGo = new GameObject("Ear");
            earGo.AddComponent<AudioListener>();
            Core.AttachToCirilla(earGo);
        }
        public void VeiwDispose()
        {
            GameObject.Destroy(earGo);
        }

        public void SetSlotColor(int index, Color color)
        {
            Image image = null;
            switch (index)
            {
                case 1:
                    image = slot1;
                    break;
                case 2:
                    image = slot2;
                    break;
                case 3:
                    image = slot3;
                    break;
                case 4:
                    image = slot4; 
                    break;
            }
            
            if (image == null)
                return;

            image.color = color;
        }
        #endregion
    }
}
