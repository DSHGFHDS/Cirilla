using Cirilla;

namespace GameLogic
{
    public class TESTController : IController
    {
        [Model] TestModel testModel;
        [View] TestView testView;
        [View] UIView uIView;
        public TESTController()
        {
             
        }

        public void Dispose()
        {
            CiriDebugger.Log("TESTController Out");
        }

        public void Init()
        {
            CiriDebugger.Log("TESTController");
        }
    }
}