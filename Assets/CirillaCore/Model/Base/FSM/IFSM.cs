
namespace Cirilla
{
    public interface IFSM<TState> : IChange<TState>
    {
        void Add<T>() where T : TState;
        void Remove<T>() where T : TState;
        void Clear();
    }
}
