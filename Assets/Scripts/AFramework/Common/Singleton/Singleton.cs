using System;

namespace AFramework.Singleton
{
    public abstract class Singleton<T> : ISingleton where T : Singleton<T> //非常死的约束，使得无法像List<int> list 这样直接当作类型使用； 必须新建一个继承自Singleton<T>的类来使用
    {
        private bool hasInit;
        private static readonly Lazy<T> instance = new Lazy<T>(() =>
        {
            var type = typeof(T);
            // 获取私有构造函数
            var constructorInfos = type.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            // 获取无参构造函数
            var ctor = Array.Find(constructorInfos, c => c.GetParameters().Length == 0);

            if (ctor == null)
            {
                throw new Exception("Non-Public Constructor() not found! in " + type);
            }

            return ctor.Invoke(null) as T;
        });

        public static T Instance
        {
            get { return instance.Value; }
        }

        public void Init()
        {
            if (!hasInit)
            {
                OnInit();
                hasInit = true;
            }
        }

        public void Clear()
        {
            if (hasInit)
            {
                OnClear();
                hasInit = false;
            }
        }

        protected virtual void OnInit() { }
        protected virtual void OnClear() { }
    }
}