using Cirilla;
using UnityEngine;

namespace GameLogic
{
    //Controllerͨ��Model���ݶ�View����в���
    public class TestController : IController
    {
        #region Model��Viewֻ������Controller��ע��
        [Model] TestModel testModel;
        [View] UITestView uiTestView;

        [Dependency] IAudioModule audioModule;
        [Dependency] IResModule resModule;
        [Dependency] IMVCModule mVCModule;
        #endregion

        private AudioClip audioClip;
        public void Init()
        {
            //��ȡ��Դ��ʵӦ�ý���һ��ר�Ż�ȡ�͹�����Դ��Model������ֱ��д��controller���洿��͵����
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