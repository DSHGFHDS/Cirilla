
using System;

namespace Cirilla
{
    public interface INetModule
    {
        void Disconnect();
        void Connect(INetBase netBase);
        void HttpRequest(string Url, Action<byte[]> callBack);
    }
}
