using Cirilla;

namespace GameLogic
{
    public class TestView : IView
    {
        public void Dispose()
        {

        }

        public void Init()
        {
            CiriDebugger.Log("TestView");
        }
    }
}
