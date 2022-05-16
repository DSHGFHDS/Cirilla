using System;
using System.Reflection;

namespace Cirilla
{
    public class MVCModule : IMVCModule
    {
        private static IContainer containerIns;

        public MVCModule() => containerIns = IocContainer.instance;

        public T Add<T>(string key) where T : class, IController => (T)Load(typeof(T), key);

        public object Add(Type type, string key)
        {
            if (!(type is IController))
                return null;

            return Load(type, key);
        }

        public void InjectController(ContentInfo contentInfo)
        {
            foreach (FieldInfo fieldInfo in contentInfo.fieldInfos)
            {
                ControllerAttribute controllerAttribute = fieldInfo.GetCustomAttribute<ControllerAttribute>();
                if (controllerAttribute == null)
                    continue;

                Type fieldType = fieldInfo.FieldType;
                if (!(typeof(IController).IsAssignableFrom(fieldType)))
                    continue;

                fieldInfo.SetValue(contentInfo.obj, Load(fieldType, controllerAttribute.key));
            }
        }

        private object Load(Type type, string key)
        {
            string finalKey = type.Name + key;
            Type mvcType = typeof(IMVCBase);
            ContentInfo contentInfo = containerIns.GetContentInfo(mvcType, finalKey);
            if (contentInfo != null)
                return contentInfo.obj;

            containerIns.Register<IMVCBase>(type, finalKey);
            IMVCBase obj = (IMVCBase)containerIns.Resolve(mvcType, finalKey);
            if (!(typeof(IController).IsAssignableFrom(type)))
            {
                obj.Init();
                return obj;
            }

            contentInfo = containerIns.GetContentInfo(mvcType, finalKey);

            foreach (FieldInfo fieldInfo in contentInfo.fieldInfos)
            {
                Type fieldType = fieldInfo.FieldType;
                ModelAttribute modelAttribute = fieldInfo.GetCustomAttribute<ModelAttribute>();
                if (modelAttribute != null)
                {
                    if (!(typeof(IModel).IsAssignableFrom(fieldType)))
                        continue;

                    fieldInfo.SetValue(obj, Load(fieldType, modelAttribute.key));
                    continue;
                }
                ViewAttribute viewAttribute = fieldInfo.GetCustomAttribute<ViewAttribute>();
                if (viewAttribute == null)
                    continue;

                if (!(typeof(IView).IsAssignableFrom(fieldType)))
                    continue;

                fieldInfo.SetValue(obj, Load(fieldType, viewAttribute.key));
            }

            obj.Init();
            return obj;
        }
    }
}
