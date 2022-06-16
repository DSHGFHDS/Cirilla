using Cirilla;
using UnityEngine;

namespace GameLogic
{
    public class TestProcess : AProcessBase
    {
        #region Controller
        [Controller] TestController testController;
        #endregion

        #region 流程初始化与释放
        public override void Init()
        {
        }
        #endregion
        #region 流程往返
        public override void OnEnter(params object[] args)
        {
        }
        public override void OnExit()
        {
        }
        #endregion
        #region 心跳帧
        public override void OnInputUpdate()
        {
            if(Input.GetKeyDown(KeyCode.W))
                testController.SetSlotWhateverColor(Slot.Slot1);

            if (Input.GetKeyDown(KeyCode.A))
                testController.SetSlotWhateverColor(Slot.Slot2);

            if (Input.GetKeyDown(KeyCode.D))
                testController.SetSlotWhateverColor(Slot.Slot3);

            if (Input.GetKeyDown(KeyCode.S))
                testController.SetSlotWhateverColor(Slot.Slot4);

            if (Input.GetKeyDown(KeyCode.F))
                testController.PlaySound();
        }

        public override void OnLogicUpdatePre()
        {
            
        }
        public override void OnLogicUpdatePost()
        {
        }
        public override void OnPhysicUpdate()
        {
        }
        #endregion
    }
}
