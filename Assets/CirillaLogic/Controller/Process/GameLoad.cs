
namespace Cirilla
{
    public class GameLoad : AProcessBase
    { 
        public override void Init()
        {
        }
        public override void OnEnter(params object[] args)
        {
            CiriDebugger.Log("这里是入口，请从这里开始自定义流程");
        }

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

        public override void OnExit()
        {
            CiriDebugger.Log("离开");
        }
    }

}