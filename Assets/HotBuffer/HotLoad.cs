using Cirilla;
using UnityEngine;

public class HotLoad
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Init()
    {
        //安卓、IOS进行StreamingAsset和persistentData比对结束后会调用以下委托;非移动端不经过处理直接调用以下方法。
        Core.loadHotBuffer = () =>
        {
            /*此处为载入前的资源检查
             * 鉴于移动端StreamingAsset不可写的情况，为了保证正常热更，安卓与IOS在启动时会先比对persistentData和StreamingAsset下的version版本文件，
             * 若StreamingAsset下的版本较高或者persistentData下不存在version文件，则会将StreamingAsset下的基础资源包括版本文件拷贝到persistentData下，
             * 完成以后会调用Core.loadHotBuffer委托，之后在此处调用Core.loadProcess()进入流程。之后资源模块只会读取persistentData中的资源。
             * PC、Mac等正常读取StreamingAsset，不会进行上述操作。
             * 若需要进行网络比对校验下载，可在这里进行编写。
            */

            //进入流程
            Core.loadProcess();
        };
    }
}

