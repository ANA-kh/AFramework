using UnityEngine;

namespace AFramework.UIModule
{
    public class UIBase
    {
        protected UISceneBase sceneBase;
        protected UILoader loader;
        /// <summary>
        /// 是否堆栈打开
        /// </summary>
        public bool IsNavigation { get; set; }

        public UIBase(UISceneBase sceneBase, UILoader loader)
        {
            this.sceneBase = sceneBase;
            this.loader = loader;
        }
    }

    public class UILoader
    {
        public virtual GameObject LoadPrefab(string path)
        {
            return Resources.Load<GameObject>(path);
        }

        public virtual void LoadPrefabAsync(string path, System.Action<GameObject> callback)
        {
            Resources.LoadAsync<GameObject>(path).completed += operation =>
            {
                var request = operation as ResourceRequest;
                callback?.Invoke(request.asset as GameObject);
            };
        }
    }

    public class UIComponent : UIBase { }

    public class UIWindow : UIBase { }

    public class UISceneBase { }

    using System;
    using System.Collections.Generic;
}