using Cirilla;
using UnityEngine;

namespace GameLogic
{
    //Controller通过Model数据对View层进行操作
    public class TestController : IController
    {
        #region Model和View只允许在Controller中注入
        [Model] TestModel testModel;
        [View] UITestView uiTestView;

        [Dependency] IAudioModule audioModule;
        [Dependency] IResModule resModule;
        [Dependency] IMVCModule mVCModule;
        #endregion

        private AudioClip audioClip;
        public void Init()
        {
            //获取资源其实应该建立一个专门获取和管理资源的Model，这里直接写在controller里面纯属偷懒。
            resModule.LoadCustom("ManualLoad_custom");
            audioClip = resModule.LoadAsset<AudioClip>("ManualLoad_custom/AssassinKill.mp3");
        }

        public void Dispose()
        {
            resModule.UnLoadCustom("ManualLoad_custom");
            mVCModule.Remove<TestModel>();
            mVCModule.Remove<UITestView>();
        }

        public void SetSlotWhateverColor(Slot slot)
        {
            uiTestView.SetSlotColor((int)slot, testModel.GetRandomClore());
        }

        public void PlaySound()
        {
            audioModule.Play(audioClip);
        }
    }
}