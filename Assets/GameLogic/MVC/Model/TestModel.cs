using Cirilla;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /*
    (TestModel)partial 这一边提供数据的获取和处理方法
    (TestService)partial 则为TestModel提供数据的来源
    */
    public partial class TestModel : IModel
    {
        private List<Color> colors;

        public Color GetRandomClore()
        {
            if (colors == null || colors.Count <= 0)
                return Color.clear;

            return colors[Random.Range(0, colors.Count)];
        }
    }
}