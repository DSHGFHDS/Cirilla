
using System.IO;
using System.Runtime.Serialization;
using Object = UnityEngine.Object;

namespace Cirilla
{
    public partial class Util
    {
        public static byte[] XmlSerialize(object obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            DataContractSerializer dcs = new DataContractSerializer(obj.GetType());
            dcs.WriteObject(memoryStream, obj);
            byte[] bytes = memoryStream.GetBuffer();
            memoryStream.Close();
            return bytes;
        }

        public static T XmlDeserialize<T>(byte[] bytes)
        {
            MemoryStream memoryStream = new MemoryStream(bytes);
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            memoryStream.Seek(0, SeekOrigin.Begin);
            T obj = (T)dcs.ReadObject(memoryStream);
            memoryStream.Close();
            return obj;
        }
    }
}
