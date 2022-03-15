
namespace Cirilla
{
    public abstract class AMonoSingletonBase<T> : ASingletonEntity where T : ASingletonEntity
    {
        private static T ins;
        public static T instance
        {
            get
            {
                if (ins == null)
                {
                    if ((ins = goInstance.GetComponent<T>()) == null)
                        ins = goInstance.AddComponent<T>();

                    return ins;
                }

                return ins;
            }
        }

        protected override abstract void Init();
    }
}