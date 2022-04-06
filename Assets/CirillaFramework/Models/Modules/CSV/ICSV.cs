
namespace Cirilla
{
    public interface ICSV
    {
        object[] GetValue<T>() where T : class;
        T GetValue<T>(object primaryKey) where T : class;
        void SetValue(object csvData);
        void LoadCSV<T>(string packageName, string assetName) where T : class;
        void LoadCSV<T>(string filePath) where T : class;
        void Clear();
    }
}
