
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;

namespace Cirilla
{
    public class LogRedirect
    {
        private static string GetSelectedStackTrace()
        {
            Assembly editorWindowAssembly = typeof(EditorWindow).Assembly;
            if (editorWindowAssembly == null)
                return null;

            System.Type consoleWindowType = editorWindowAssembly.GetType("UnityEditor.ConsoleWindow");
            if (consoleWindowType == null)
                return null;

            FieldInfo consoleWindowFieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            if (consoleWindowFieldInfo == null)
                return null;

            EditorWindow consoleWindow = consoleWindowFieldInfo.GetValue(null) as EditorWindow;
            if (consoleWindow == null)
                return null;

            if (consoleWindow != EditorWindow.focusedWindow)
                return null;

            FieldInfo activeTextFieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            if (activeTextFieldInfo == null)
                return null;

            return (string)activeTextFieldInfo.GetValue(consoleWindow);
        }

        [OnOpenAssetAttribute(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            string trace = GetSelectedStackTrace();
            if (trace == null)
                return false;

            string[] buffer = trace.Split(new[] { CiriDebugger.logID + "\n" }, System.StringSplitOptions.None);

            if (buffer.Length < 2)
                return false;

            buffer = buffer[1].Split(new [] { "\n" }, System.StringSplitOptions.None);
            InternalEditorUtility.OpenFileAtLineExternal(buffer[0], int.Parse(buffer[1]));
            
            return true;
        }
    }
}