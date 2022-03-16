using UnityEngine;
using UnityEditor;

namespace Cirilla
{
    public class CreateEntrance
    {
        [MenuItem("GameObject/Cirilla/启动入口", false, 13)]
        public static void CreateManager()
        {
            ProcessManager.instance.gameObject.AddComponent<GlobalData>();
            Debug.Log("游戏入口已创建");
        }
    }
}
