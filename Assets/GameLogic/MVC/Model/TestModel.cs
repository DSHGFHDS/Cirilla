using Cirilla;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /*为了展示MVC的一个设计规范，model拆分成了许多粒度的object,实际unity的开发中很难看到完全按照规范拆分粒度的做法，这里就纯属演示，是否采用规范要根据业务实际情况来*/
    public partial class TestModel : IModel
    {
        private List<Color> colors;

        public ShowColor GetRandomClore()
        {
            if (colors == null || colors.Count <= 0)
                return new ShowColor(Color.clear);

            return new ShowColor(colors[Random.Range(0, colors.Count)]);
        }
    }
}