
namespace Cirilla
{
    public interface IChange<TState>
    {
        void Change<T>(params object[] args) where T : TState;
    }
}
