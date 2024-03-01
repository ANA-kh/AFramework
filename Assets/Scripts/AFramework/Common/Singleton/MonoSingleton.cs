using UnityEngine;

namespace AFramework.Singleton
{
    public abstract class MonoSingleton<T> : UnityEngine.MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        private bool hasInit;
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        var go = new GameObject("Singleton of " + typeof(T).ToString(), typeof(T))
                        {
                            //对象不会保存到场景中。加载新场景时不会被销毁。相当于HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset
                            hideFlags = HideFlags.DontSave
                        };

                        instance = go.GetComponent<T>();
                        instance.Init();
                    }
                }

                return instance;
            }
        }

        public void Init()
        {
            if (hasInit == false)
            {
                OnInit();
                hasInit = true;
            }
        }

        protected virtual void OnInit() { }

        public void Clear()
        {
            if (hasInit)
            {
                DestroyImmediate(gameObject);
                hasInit = false;
            }
        }
    }
}