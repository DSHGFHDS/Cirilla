
namespace Cirilla
{
    public interface IFSM
    {
        void Add<T>() where T : AStateBase;
        void Remove<T>() where T : AStateBase;
        void Change<T>(params object[] args) where T : AStateBase;
        void OnUpdate();
        void Clear();
    }
}
