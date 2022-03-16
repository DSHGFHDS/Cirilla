using System.Diagnostics;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace Cirilla
{
    public class CiriDebugger
    {
        public static readonly string logID = "------------------------------------CirillaLogger------------------------------------";
        public static void Log(string message, LogColor color = LogColor.Pink)
        {
#if UNITY_EDITOR
            Debug.Log(FormatMessage(message, color));
#endif
        }

        public static void LogError(string message, LogColor color = LogColor.Red)
        {
#if UNITY_EDITOR
            Debug.LogError(FormatMessage(message, color));
#endif
        }
        public static void LogWarning(string message, LogColor color = LogColor.Yellow)
        {
#if UNITY_EDITOR
            Debug.LogWarning(FormatMessage(message, color));
#endif
        }

        private static string FormatMessage(string message, LogColor color)
        {
            StackFrame sf = new StackTrace(true).GetFrame(2);
            MethodBase mb = sf.GetMethod();
            int line = +sf.GetFileLineNumber();

            if (message == null)
                message = "Message is null";
            else if (message.Length <= 0)
                message = "Message is empty";

            message += "</color>\n" + mb.DeclaringType.FullName + "." + sf.GetMethod().Name + "|Line:" + line + "\n" + logID + "\n" + sf.GetFileName() + "\n" + line;

            FieldInfo fieldInfo = color.GetType().GetField(color.ToString());
            AttrColor ANet = fieldInfo.GetCustomAttribute<AttrColor>();

            return "<color=" + ANet.color + ">" + message;
        }
    }
}
