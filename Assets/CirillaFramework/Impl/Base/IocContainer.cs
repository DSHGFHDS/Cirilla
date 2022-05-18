
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cirilla
{
    public class IocContainer : ASingletonBase<IocContainer>, IContainer
    {
        private const BindingFlags Flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        private Dictionary<Type, Dictionary<string, ContentInfo>> stock;
        
        public IocContainer() => stock = new Dictionary<Type, Dictionary<string, ContentInfo>>();

        private void Inject(ContentInfo contentInfo)
        {
            Object obj = contentInfo.obj;
            Type type = obj.GetType();
            FieldInfo[] fieldInfo = type.GetFields(Flag);
            for (int i = 0; i < fieldInfo.Length; i++)
            {
                contentInfo.fieldInfos.Add(fieldInfo[i]);
                DependencyAttribute attribute = fieldInfo[i].GetCustomAttribute<DependencyAttribute>();
                if (attribute == null)
                    continue;
                
                Type fieldType = fieldInfo[i].FieldType;

                object target = Resolve(fieldType, attribute.key);
                if(target == null)
                {
                    CiriDebugger.Log("Inject failed:" + type.Name);
                    continue;
                }

                fieldInfo[i].SetValue(obj, target);
            }
        }
        private void Register<T>(string key, ContentInfo contentInfo)
        {
            Type type = typeof(T);
            if (stock.TryGetValue(type, out Dictionary<string, ContentInfo> contentInfos))
            {
                if (contentInfos.ContainsKey(key))
                    return;

                contentInfos.Add(key, contentInfo);

                return;
            }

            stock.Add(type, new Dictionary<string, ContentInfo>() { { key, contentInfo } });
        }

        public void Register<T>(Type type, string key) => Register<T>(key, new ContentInfo(type, key, null));

        public void Register<T1, T2>(string key) where T1 : class where T2 : T1 => Register<T1>(key, new ContentInfo(typeof(T2), key, null));

        public void Register<T>(T instance, string key) => Register<T>(key, new ContentInfo(instance.GetType(), key, instance));

        public void Unregister<T>(string key)
        {
            Type type = typeof(T);
            if (!stock.TryGetValue(type, out Dictionary<string, ContentInfo> contentInfos))
                return;

            if (!contentInfos.ContainsKey(key))
                return;

            contentInfos.Remove(key);
        }

        public T Resolve<T>(string key) where T : class
        {
            Type type = typeof(T);
            return (T)Resolve(type, key);
        }

        public object Resolve(Type type, string key)
        {
            if (!stock.TryGetValue(type, out Dictionary<string, ContentInfo> contentInfos))
            {
                CiriDebugger.Log("ÀàÐÍÎ´×¢²á:" + type);
                return null;
            }

            if (!contentInfos.TryGetValue(key, out ContentInfo contentInfo))
            {
                CiriDebugger.Log("¼üÖµÎ´×¢²á:" + key);
                return null;
            }

            object obj = contentInfo.obj;
            if (contentInfo.IsInjected)
                return obj;

            Inject(contentInfo);
            contentInfo.IsInjected = true;

            return obj;
        }

        public ContentInfo GetContentInfo<T>(string key) where T : class
        {
            Type type = typeof(T);
            return GetContentInfo(type, key);
        }

        public ContentInfo GetContentInfo(Type type, string key)
        {
            if (!stock.TryGetValue(type, out Dictionary<string, ContentInfo> contentInfos))
                return null;

            if (!contentInfos.TryGetValue(key, out ContentInfo contentInfo))
                return null;

            return contentInfo;
        }

        public ContentInfo[] GetContentInfos<T>()
        {
            Type matchType = typeof(T);

            if (!stock.TryGetValue(matchType, out Dictionary<string, ContentInfo> contentInfos))
                return null;

            return new List<ContentInfo>(contentInfos.Values).ToArray();
        }

        public ContentInfo[] GetContentInfos()
        {
            List<ContentInfo> contentInfosBuffer = null;
            foreach (Dictionary<string, ContentInfo> contentInfos in stock.Values)
                contentInfosBuffer = new List<ContentInfo>(contentInfos.Values);
            return contentInfosBuffer?.ToArray();
        }

        public void Clear() => stock.Clear();

    }
}
