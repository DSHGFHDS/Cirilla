using Cirilla;

namespace GameLogic
{
    public class TestModel : IModel
    {
        public void Dispose()
        {
        }

        public void Init()
        {
            CiriDebugger.Log("TestModel");
        }
    }
}