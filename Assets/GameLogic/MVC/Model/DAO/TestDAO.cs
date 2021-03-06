using Cirilla;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public partial class TestModel : IModel
    {
        public void Init()
        {
            colors = new List<Color>();
            LoadData();
        }

        public void Dispose()
        {
            colors.Clear();
        }

        //假设以下数据来源于数据库、csv、js等本地表
        private void LoadData()
        {
            colors.Add(Color.yellow);
            colors.Add(Color.red);
            colors.Add(Color.blue);
            colors.Add(Color.green);
            colors.Add(Color.gray);
            colors.Add(Color.cyan);
        }
    }
}