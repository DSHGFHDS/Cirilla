using Cirilla;
using UnityEngine;

namespace GameLogic
{
    public class TestController : IController
    {
        #region Model��Viewֻ������Controller��ע��
        [Model] TestModel testModel;
        [View] TestUIView testUIView;
        [Dependency] IAudioModule audioModule;
        [Dependency] IResModule resModule;
        #endregion

        public void Dispose()
        {
            CiriDebugger.Log("TestController Out");
        }

        public void Init()
        {
            CiriDebugger.Log("TestController In");

            AudioClip audioClip = resModule.LoadAsset<AudioClip>("AssassinKill.mp3");

            audioModule.Play(audioClip, false, 100);
        }
    }
}