using System.Collections.Generic;

namespace AFramework.Editor.Builder.BuildContext
{
    public class BuildContext
    {
        private Dictionary<System.Type, object> _contextObjects = new Dictionary<System.Type, object>();

        public T GetContextObject<T>()
        {
            var type = typeof(T);
            if (_contextObjects.TryGetValue(type, out object contextObject))
            {
                return (T)contextObject;
            }
            else
            {
                throw new System.Exception($"Not found context object : {type}");
            }
        }

        public void SetContextObject(object contextObject)
        {
            if (contextObject == null)
                throw new System.ArgumentNullException("contextObject");

            var type = contextObject.GetType();
            if (_contextObjects.ContainsKey(type))
                throw new System.Exception($"Context object {type} is already existed.");

            _contextObjects.Add(type, contextObject);
        }
    }
}