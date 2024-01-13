using System.Collections.Generic;
using System.Xml.Serialization;

namespace AFramework.ResModule
{
    public abstract class ResManager : IResManager
    {
        protected Dictionary<string, Res> _resMap = new Dictionary<string, Res>();

        public abstract Res Load(string path);

        public abstract Res LoadAsync(string path);

        public void Retain(Res res)
        {
            if (!_resMap.TryGetValue(res.Key(), out var res1))
            {
                _resMap.Add(res.Key(), res);
            }
        }

        public void Release(Res res)
        {
            if (_resMap.TryGetValue(res.Key(), out var res1))
            {
                _resMap.Remove(res.Key());
            }

            //TODO: LRU
            res.Dispose();
        }
    }
}