using Cirilla;

namespace GameLogic
{
    public class TESTController : IController
    {
        [Model] TestModel testModel;
        [View] TestView testView;
        public TESTController()
        {

        }

        public void Dispose()
        {
        }

        public void Init()
        {
            CiriDebugger.Log("TESTController");
        }
    }
}