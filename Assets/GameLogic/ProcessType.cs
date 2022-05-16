using Cirilla;

namespace GameLogic
{
   public enum ProcessType
   {
        [ProcessInfoAttribute(typeof(TestProcess), true)]
        TestProcess,
   }
}
