using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Cirilla
{
    public class NetModule : INet
    {
        private INetBase netHandle;
        private List<NetInfoAttribute> netInfos;
        public NetModule()
        {
            Type type = Util.GetTypeFromName("NetType", "GameLogic");
            netInfos = new List<NetInfoAttribute>();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                NetInfoAttribute attribute = fieldInfo.GetCustomAttribute<NetInfoAttribute>();
                if (attribute == null)
                    continue;

                netInfos.Add(attribute);
            }
        }

        public void Connect(int selected)
        {
            if (netHandle != null)
                netHandle.Disconnect();

            netHandle = (INetBase)Activator.CreateInstance(netInfos[selected].type, netInfos[selected].url, 1024 * 8);

            netHandle.Connect();
        }

        public void Disconnect()
        {
            if (netHandle == null)
                return;

            netHandle.Disconnect();
        }

        public async void HttpRequest(string Url, Action<byte[]> callBack)
        {
            if (string.IsNullOrEmpty(Url))
                return;

            byte[] bytes = await Task<byte[]>.Run(() =>
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
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
