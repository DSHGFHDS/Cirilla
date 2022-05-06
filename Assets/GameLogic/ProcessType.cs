using Cirilla;

namespace GameLogic
{
   public enum ProcessType
   {
        [ProcessInfoAttribute(typeof(NewProcess), true)]
        NewProcess,
   }
}
