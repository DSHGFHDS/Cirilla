
using System;

namespace Cirilla
{
    public interface INet
    {
        void Disconnect();
        void Connect(int selected);
        void HttpRequest(string Url, Action<byte[]> callBack);
    }
}
