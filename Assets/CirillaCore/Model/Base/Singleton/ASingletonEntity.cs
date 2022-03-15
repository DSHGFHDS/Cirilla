
using UnityEngine;

namespace Cirilla
{
    public class ASingletonEntity : MonoBehaviour
    {
        private const string gameEntname = "GameEntity";
        private static GameObject go;
        protected static GameObject goInstance
        {
            get
            {
                if (go == null)
                {
                    if ((go = GameObject.Find(gameEntname)) == null)
                    {
                        go = new GameObject(gameEntname);
                        go.AddComponent<ASingletonEntity>();
                    }
                }

                return go;
            }
        }

        protected void Awake()
        {
            if (go != null && go != gameObject)
            {
                Destroy(gameObject);
                return;
            }

            go = gameObject;
            DontDestroyOnLoad(go);
            Init();
        }

        protected virtual void Init(){
        }
    }
}
