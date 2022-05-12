
using System.Threading.Tasks;
using UnityEngine;

namespace Cirilla
{
    public interface ICSVModule
    {
        T[] Load<T>(TextAsset textAsset) where T : class;
        Task<T[]> Load<T>(string path) where T : class;
        void Write<T>(T[] csvDatas, string path);
    }
}
