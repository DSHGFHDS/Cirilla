using Cirilla;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /*Ϊ��չʾMVC��һ����ƹ淶��model��ֳ���������ȵ�object,ʵ��unity�Ŀ����к��ѿ�����ȫ���չ淶������ȵ�����������ʹ�����ʾ���Ƿ���ù淶Ҫ����ҵ��ʵ�������*/
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