
namespace Cirilla
{
    public interface IState<T> : IGameUpdate, IChange<T>
    {
        void Init();
        void OnEnter(params object[] args);
        void OnExit();
    }
}
