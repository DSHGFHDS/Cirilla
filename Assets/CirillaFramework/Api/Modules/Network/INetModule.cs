
using System;

namespace Cirilla
{
    public interface INetModule
    {
        void Disconnect();
        void Connect(int selected);
        void HttpRequest(string Url, Action<byte[]> callBack);
    }
}
