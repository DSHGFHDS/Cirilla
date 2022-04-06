using System;
using System.Reflection;

namespace Cirilla
{
    public partial class Util
    {
        public static Type GetTypeFromName(string name, string assemblyName = "")
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!string.IsNullOrEmpty(assemblyName) && assembly.GetName().Name != assemblyName)
                    continue;

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name != name)
                        continue;
                    return type;
                }

                if (!string.IsNullOrEmpty(assemblyName))
                    return null;
            }

            return null;
        }
    }
}
