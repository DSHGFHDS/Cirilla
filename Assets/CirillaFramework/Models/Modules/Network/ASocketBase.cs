
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Cirilla
{
    public abstract class ASocketBase: INetBase
    {
        public string ip { get; private set; }
        public int port { get; private set; }
        public int bufferLen;
        public bool isConnected { get { return socket == null; } }
        private Socket socket;
        protected ASocketBase(string ip, int port, int bufferLen)
        {
            this.ip = ip;
            this.port = port;
            this.bufferLen = bufferLen;
        }

        public async void Connect()
        {
            if (socket != null)
            {
                CiriDebugger.LogWarning("Already connected");
                return;
            }

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await socket.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
            }
            catch
            {
                CiriDebugger.LogWarning("Connect failed");
                socket.Close();
                socket = null;
                return;
            }

            Connected();

            _ =Task.Run(() =>
            {
                try
                {
                    byte[] buffer = new byte[bufferLen];
                    while (socket.Receive(buffer) > 0) {
                        CirillaCore.Push(Received, buffer);
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
            Disconnected();
        }
        protected abstract void Connected();
        protected abstract void Received(params object[] args);
        protected abstract void Disconnected();
    }
}
