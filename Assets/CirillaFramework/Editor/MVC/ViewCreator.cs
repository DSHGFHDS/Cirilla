using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Cirilla.CEditor
{
    public class ViewCreator
    {
        private const string UIView = "UIView";

        [MenuItem("GameObject/CirillaView/UIView", false, -1)]
        private static void CreateUIView()
        {
            GameObject view = new GameObject(UIView);
            view.AddComponent<ViewEntity>();
            view.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler canvasScaler = view.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            view.AddComponent<GraphicRaycaster>();
        }

        [MenuItem("GameObject/CirillaView/NormalView", false, -1)]
        private static void CreateNormalView()
        {

        }
        /*
        private static bool WriteView()
        {
            string path;
            if (EditorUtil.devPath == string.Empty || !Directory.Exists(path = Application.dataPath + "/" + EditorUtil.devPath.Substring("Assets/".Length)))
            {
                EditorGUILayout.HelpBox("请在项目配置表中设置开发目录", MessageType.Warning);
                return;
            }

            string processFile = path += "/ProcessType.cs";
        }*/
    }
}
