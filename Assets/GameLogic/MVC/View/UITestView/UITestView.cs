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

        public void SetSlotColor(int index, ShowColor showColor)
        {
            //C#8.0 switch(index) 变形 语法糖
            Image image = index switch
            {
                1 => slot1,
                2 => slot2,
                3 => slot3,
                4 => slot4,
                _ =>null
            };

            if (image == null)
                return;

            image.color = showColor.color;
        }
        #endregion
    }
}
