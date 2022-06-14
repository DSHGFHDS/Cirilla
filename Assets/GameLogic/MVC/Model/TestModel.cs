using Cirilla;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /*
    (TestModel)partial ��һ���ṩ���ݵĻ�ȡ�ʹ�����
    (TestService)partial ��ΪTestModel�ṩ���ݵ���Դ
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