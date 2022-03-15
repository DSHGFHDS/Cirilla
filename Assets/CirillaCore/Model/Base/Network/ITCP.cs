
namespace Cirilla
{
    public interface ITCP
    {
        void Connect();
        void Disconnect(byte[] notice);
        bool Send(byte[] netPackge);
    }
}
