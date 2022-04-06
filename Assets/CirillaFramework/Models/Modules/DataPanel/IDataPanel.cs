
namespace Cirilla
{
    public interface IDataPanel
    {
        T GetValue<T>(string globalKey, int index = 0);
        T GetValue<T>(string configName, string key, int index = 0);
        T[] GetValues<T>(string configName, string key);
        void Load(string packageName, string configName);
        void Load(string resourcePath);
        void Load(DataPanel configAsset);
        void Remove(string configNameOrPath);
        void Clear();
    }
}
