
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cirilla
{
    public class IocContainer : ASingletonBase<IocContainer>, IContainer
    {
        private const BindingFlags Flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        private const string originKey = "";
        private Dictionary<Type, Dictionary<string, TypeInfo>> stock;
        
        public IocContainer()
        {
            stock = new Dictionary<Type, Dictionary<string, TypeInfo>>();
        }

        private void Inject(object obj)
        {
            Type type = obj.GetType();
            FieldInfo[] fieldInfo = type.GetFields(Flag);
            for (int i = 0; i < fieldInfo.Length; i++)
            {
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
        
        private void Register<T>(string key, TypeInfo typeInfo)
        {
            Type type = typeof(T);
            if (stock.TryGetValue(type, out Dictionary<string, TypeInfo> typeInfos))
            {
                if (typeInfos.ContainsKey(key))
                    return;

                typeInfos.Add(key, typeInfo);

                return;
            }

            stock.Add(type, new Dictionary<string, TypeInfo>() { { key, typeInfo } });
        }

        public void Register<T>(Type type, string key = originKey){
            Register<T>(key, new TypeInfo(type, null));
        }

        public void Register<T1, T2>(string key = originKey) where T1 : class where T2 : T1{
            Register<T1>(key, new TypeInfo(typeof(T2), null));
        }

        public void Register<T>(T instance, string key = originKey){
            Register<T>(key, new TypeInfo(instance.GetType(), instance));
        }

        public void Unregister<T>(string key = originKey)
        {
            Type type = typeof(T);
            if (!stock.TryGetValue(type, out Dictionary<string, TypeInfo> typeInfos))
                return;

            if (!typeInfos.ContainsKey(key))
                return;

            typeInfos.Remove(key);
        }

        public T Resolve<T>(string key = originKey) where T : class
        {
            Type type = typeof(T);
            return (T)Resolve(type, key);
        }

        public object Resolve(Type type, string key = originKey)
        {
            if (!stock.TryGetValue(type, out Dictionary<string, TypeInfo> typeInfos))
            {
                CiriDebugger.Log("Unregister Type:" + type);
                return null;
            }

            if (!typeInfos.TryGetValue(key, out TypeInfo typeInfo))
            {
                CiriDebugger.Log("Unregister key:" + key);
                return null;
            }

            object obj = typeInfo.instance;
            if (typeInfo.IsInjected)
                return obj;

            Inject(obj);
            typeInfo.IsInjected = true;

            return obj;
        }

        public void Clear() {
            stock.Clear();
        }
    }
}
