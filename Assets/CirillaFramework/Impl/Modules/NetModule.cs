using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Cirilla
{
    public class NetModule : INetModule
    {
        private INetBase netHandle;

        public NetModule()
        {
        }

        public void Connect(INetBase netHandle)
        {
            if (netHandle != null)
                netHandle.Disconnect();

            this.netHandle = netHandle;
            netHandle.Connect();
        }

        public void Disconnect()
        {
            if (netHandle == null)
                return;

            netHandle.Disconnect();
        }

        public async void HttpRequest(string Url, Action<byte[]> callBack, string method)
        {
            if (string.IsNullOrEmpty(Url))
                return;

            byte[] bytes = await Task.Run(() =>
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
                webRequest.Method = method;

                HttpWebResponse res = (HttpWebResponse)webRequest.GetResponse();

                if (res.StatusCode != HttpStatusCode.OK)
                {
                    res.Close();
                    return null;
                }

                Stream stream = res.GetResponseStream();
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                byte[] bytes = memoryStream.GetBuffer();
                res.Close();
                stream.Close();
                memoryStream.Close();
                return bytes;
            });

            callBack(bytes);
        }
    }
}
