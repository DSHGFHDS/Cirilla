using Cirilla;

namespace GameLogic
{
    public class TestView : IView
    {
        public void Dispose()
        {
            CiriDebugger.Log("TestView Out");
        }

        public void Init()
        {
            CiriDebugger.Log("TestView");
        }
    }
}
