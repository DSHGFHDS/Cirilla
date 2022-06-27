using System.Diagnostics;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace Cirilla
{
    public class CiriDebugger
    {
        public static readonly string logID = "------------------------------------CirillaLogger------------------------------------";
        public static void Log(object message, LogColorType color = LogColorType.Pink)
        {
            Debug.Log(FormatMessage(message, color));
        }

        public static void LogError(object message, LogColorType color = LogColorType.Red)
        {
            Debug.LogError(FormatMessage(message, color));
        }
        public static void LogWarning(object message, LogColorType color = LogColorType.Yellow)
        {
            Debug.LogWarning(FormatMessage(message, color));
        }

        private static string FormatMessage(object message, LogColorType color)
        {
            string strMsg = message?.ToString();
            StackFrame sf = new StackTrace(true).GetFrame(2);
            MethodBase mb = sf.GetMethod();
            int line = +sf.GetFileLineNumber();

            if (strMsg == null)
                strMsg = "Message is null";
            else if (strMsg.Length <= 0)
                strMsg = "Message is empty";

            strMsg += "</color>\n" + mb.DeclaringType.FullName + "." + sf.GetMethod().Name + "|Line:" + line + "\n" + logID + "\n" + sf.GetFileName() + "\n" + line;

            FieldInfo fieldInfo = color.GetType().GetField(color.ToString());
            LogColorAttribute ANet = fieldInfo.GetCustomAttribute<LogColorAttribute>();

            return "<color=" + ANet.color + ">" + strMsg;
        }
    }
}
