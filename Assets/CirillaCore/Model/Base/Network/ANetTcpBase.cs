
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Cirilla
{
    public abstract class ANetTcpBase: ITCP
    {
        private string ip;
        private int port;
        private int bufferLen;
        private Socket socket;
        private ObserverManager<NetEvent> observer;
        protected ANetTcpBase(string ip, int port, int bufferLen)
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
                    byte[] BTBuffer = new byte[bufferLen];
                    while (socket.Receive(BTBuffer) > 0) {
                        observer.Dispatch(NetEvent.Received, BTBuffer);
                    }
                }
                catch {
                    CiriDebugger.LogWarning("Connection interrupt");
                    Close();
                }
            }
            );
        }

        public void Disconnect(byte[] notice)
        {
            if (socket == null)
            {
                CiriDebugger.LogWarning("Already disconnected");
                return;
            }

            Send(notice);
            Close();
        }

        public bool Send(byte[] netPackge)
        {
            if (socket == null)
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
