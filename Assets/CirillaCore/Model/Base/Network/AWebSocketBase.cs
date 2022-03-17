using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Cirilla
{
    public class AWebSocketBase : INet
    {
        public string ip { get; private set; }
        public int port { get; private set; }
        public int bufferLen { get; private set; }
        private ClientWebSocket webSocket;
        private ObserverManager<NetEvent> observer;
        public AWebSocketBase(string ip, int port, int bufferLen)
        {
            this.ip = ip;
            this.port = port;
            this.bufferLen = bufferLen;
            webSocket = new ClientWebSocket();
            observer = ObserverManager<NetEvent>.instance;
        }

        public void Connect()
        {
            if (webSocket.State == WebSocketState.Open)
            {
                CiriDebugger.LogWarning("Already connected");
                return;
            }

            Task.Run(async () =>
              {
                  try
                  {
                      await webSocket.ConnectAsync(new Uri($"ws://{ip}:{port}"), CancellationToken.None);
                  }
                  catch {
                      CiriDebugger.LogWarning("Connect failed");
                      return;
                  }

                  observer.Dispatch(NetEvent.Connected);

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
                                  observer.Dispatch(NetEvent.ReceiveByte, bytes);
                                  break;
                              case WebSocketMessageType.Text:
                                  observer.Dispatch(NetEvent.ReceiveString, bytes);
                                  break;
                              case WebSocketMessageType.Close:
                                  await webSocket.CloseAsync(WebSocketCloseStatus.Empty, String.Empty, CancellationToken.None);
                                  observer.Dispatch(NetEvent.Disconnected);
                                  return;
                          }
                      }
                  }
                  catch
                  {
                      CiriDebugger.LogWarning("Connection interrupt");
                      await webSocket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, String.Empty, CancellationToken.None);
                      observer.Dispatch(NetEvent.Disconnected);
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

            await webSocket.SendAsync(ArraySegment<byte>.Empty, WebSocketMessageType.Close, true, CancellationToken.None);
            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, String.Empty, CancellationToken.None);
            observer.Dispatch(NetEvent.Disconnected);
        }

        public async Task<bool> Send(byte[] netPackge, bool isBinary = true)
        {
            if (webSocket.State != WebSocketState.Open || netPackge == null)
            {
                CiriDebugger.LogWarning("Send failed");
                return false;
            }

            try {
                await webSocket.SendAsync(new ArraySegment<byte>(netPackge), isBinary ? WebSocketMessageType.Binary : WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            catch {
                await webSocket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, String.Empty, CancellationToken.None);
                observer.Dispatch(NetEvent.Disconnected);
                return false;
            }
        }
    }
}
