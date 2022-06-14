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
            Image go = null;
            switch (index)
            {
                case 1:
                    go = slot1.GetComponent<Image>();
                    break;
                case 2:
                    go = slot2.GetComponent<Image>();
                    break;
                case 3:
                    go = slot3.GetComponent<Image>();
                    break;
                case 4:
                    go = slot4.GetComponent<Image>(); 
                    break;
            }

            if (go == null)
                return;

            go.color = color;
        }
        #endregion
    }
}
