using UnityEngine;
using UnityEditor;

namespace Cirilla
{
    public class CreateEntrance
    {
        [MenuItem("GameObject/Cirilla/启动入口", false, 13)]
        public static void CreateManager()
        {
            _ = ProcessManager.instance;
            Debug.Log("游戏入口已创建");
        }
    }
}
