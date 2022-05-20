using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Cirilla.CEditor
{
    public class ViewCreator
    {
        private const string UIView = "UIView";
        private const string ObjectView = "ObjectView";

        [MenuItem("GameObject/CirillaView/ObjectView", false, -1)]
        private static void CreateObjectView()
        {
            GameObject view = new GameObject(ObjectView);
            view.AddComponent<ViewEntity>();
        }

        [MenuItem("GameObject/CirillaView/UIView")]
        private static void CreateUIView()
        {
            GameObject view = new GameObject(UIView);
            view.AddComponent<ViewEntity>();
            view.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler canvasScaler = view.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            view.AddComponent<GraphicRaycaster>();
        }
    }
}
