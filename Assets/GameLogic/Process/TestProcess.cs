using Cirilla;
using UnityEngine;

namespace GameLogic
{
    public class TestProcess : AProcessBase
    {
        [Controller] TESTController testController;
        [Dependency] IResModule resModule;
        [Dependency] ICSVModule csvModule;
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
