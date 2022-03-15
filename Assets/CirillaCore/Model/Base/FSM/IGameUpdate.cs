
namespace Cirilla
{
    public interface IGameUpdate
    {
        void OnInputUpdate();
        void OnPhysicUpdate();
        void OnLogicUpdatePre();
        void OnLogicUpdatePost();
    }
}
