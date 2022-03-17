
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Cirilla
{
    public abstract class ASocketBase: INet
    {
        public string ip { get; private set; }
        public int port { get; private set; }
        public int bufferLen;

        private Socket socket;
        private ObserverManager<NetEvent> observer;
        protected ASocketBase(string ip, int port, int bufferLen)
        {
            this.ip = ip;
            this.port = port;
            this.bufferLen = bufferLen;
            observer = ObserverManager<NetEvent>.instance;
        }

        public void Connect()
        {
            if (socket != null)
            {
                CiriDebugger.LogWarning("Already connected");
                return;
            }

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Task.Run(() =>
            {
                try {
                    socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                }
                catch
                {
                    CiriDebugger.LogWarning("Connect failed");
                    socket.Close();
                    socket = null;
                    return;
                }

                observer.Dispatch(NetEvent.Connected);

                try
                {
                    byte[] buffer = new byte[bufferLen];
                    while (socket.Receive(buffer) > 0) {
                        observer.Dispatch(NetEvent.ReceiveByte, buffer);
                    }
                }
                catch {
                    CiriDebugger.LogWarning("Connection interrupt");
                    Close();
                }
            }
            );
        }

        public void Disconnect()
        {
            if (socket == null)
            {
                CiriDebugger.LogWarning("Already disconnected");
                return;
            }

            Close();
        }

        public bool Send(byte[] netPackge)
        {
            if (socket == null || netPackge == null)
            {
                CiriDebugger.LogWarning("Send failed");
                return false;
            }

            try
            {
                socket.Send(netPackge);
                return true;
            }
            catch
            {
                CiriDebugger.LogWarning("Connection lost");
                Close();
                return false;
            }
        }

        private void Close()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;
            observer.Dispatch(NetEvent.Disconnected);
        }
    }
}
