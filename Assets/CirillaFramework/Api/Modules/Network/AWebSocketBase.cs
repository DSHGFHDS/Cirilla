using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Cirilla
{
    public abstract class AWebSocketBase : INetBase
    {
        public int bufferLen { get; private set; }
        public string url;
        public bool isConnected { get { return webSocket != null; } }

        private const string normalClosureKey = "WEBSOCKET_NORMAL_CLOSE";
        private ClientWebSocket webSocket;
        public AWebSocketBase(string url, int bufferLen)
        {
            this.bufferLen = bufferLen;
            this.url = url;
            webSocket = new ClientWebSocket();
        }

        public async void Connect()
        {
            if (webSocket.State == WebSocketState.Open)
            {
                CiriDebugger.LogWarning("Already connected");
                return;
            }

            try
            {
                await webSocket.ConnectAsync(new Uri(url), CancellationToken.None);
            }
            catch
            {
                CiriDebugger.LogWarning("Connect failed");
                return;
            }

            Connected();

            _ =Task.Run(async () =>
              {
                  try
                  {
                      ArraySegment<byte> segment = new ArraySegment<byte>(new byte[bufferLen]);
                      while (true)
                      {
                          WebSocketReceiveResult result = await webSocket.ReceiveAsync(segment, CancellationToken.None);
                          byte[] bytes = new byte[result.Count];
                          Array.ConstrainedCopy(segment.Array, 0, bytes, 0, result.Count);

                          switch (result.MessageType)
                          {
                              case WebSocketMessageType.Binary:
                                  Core.Push(Received, bytes);
                                  break;
                              case WebSocketMessageType.Text:
                                  Core.Push(Received, System.Text.Encoding.UTF8.GetString(bytes));
                                  break;
                              case WebSocketMessageType.Close:
                                  if (result.CloseStatusDescription == normalClosureKey)
                                      break;
                                      Close();
                                  break;
                          }
                      }
                  }
                  catch
                  {
                      CiriDebugger.LogWarning("Connection interrupt");
                      Close();
                  }
               });
        }

        public async void Disconnect()
        {
            if (webSocket.State != WebSocketState.Open)
            {
                CiriDebugger.LogWarning("Already disconnected");
                return;
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, normalClosureKey, CancellationToken.None);
            Disconnected();
        }

        public bool Send(byte[] netPackge, bool isBinary = true)
        {
            if (webSocket.State != WebSocketState.Open || netPackge == null)
            {
                CiriDebugger.LogWarning("Send failed");
                return false;
            }

            try {
                webSocket.SendAsync(new ArraySegment<byte>(netPackge), isBinary ? WebSocketMessageType.Binary : WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            catch {
                Close();
                return false;
            }
        }

        private async void Close()
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, String.Empty, CancellationToken.None);
            Disconnected();
        }

        protected abstract void Connected();
        protected abstract void Received(params object[] args);
        protected abstract void Disconnected();
    }
}
