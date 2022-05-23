using Cirilla;

namespace GameLogic
{
    public class TestModel : IModel
    {
        public void Dispose()
        {
            CiriDebugger.Log("TestModel Out");
        }

        public void Init()
        {
            CiriDebugger.Log("TestModel In");
        }
    }
}