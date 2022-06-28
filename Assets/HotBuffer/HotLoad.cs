using Cirilla;
using UnityEngine;

public class HotLoad
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Init()
    {
        Core.loadHotBuffer = () =>
        {
            Core.loadProcess();
        };
    }
}

