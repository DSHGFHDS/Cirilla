using Cirilla;

namespace GameLogic
{
    public partial class ObjectView : IView
    {
        #region 初始化与释放
        public void Init()
        {
               Load();
        }
        public void Dispose()
        {
        }
        #endregion
    }
}
