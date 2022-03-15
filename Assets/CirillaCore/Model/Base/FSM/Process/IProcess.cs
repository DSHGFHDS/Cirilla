
namespace Cirilla
{
    public interface IProcess : IFSM<AProcessBase>
    {
        void ProcessGC();
        void AppQuit();
    }
}
