using Cirilla;

public class GameInit : AProcessBase
{
    public override void Init()
    {

    }

    public override void OnEnter(params object[] args)
    {
        CiriDebugger.Log("这里是入口");
    }

    public override void OnExit()
    {
    }

    public override void OnInputUpdate()
    {
    }

    public override void OnLogicUpdatePost()
    {
    }

    public override void OnLogicUpdatePre()
    {
    }

    public override void OnPhysicUpdate()
    {
    }
}
