using System.Collections.Generic;
using System.Xml.Serialization;

namespace AFramework.ResModule
{
    public class LocalResManager : ResManager
    {
        public override Res Load(string path)
        {
            _resMap.TryGetValue(path, out var res);
            if (res == null)
            {
                res = new LocalRes(path, this);
                res.Load();
            }

            return res;
        }

        public override Res LoadAsync(string path)
        {
            _resMap.TryGetValue(path, out var res);
            if (res == null)
            {
                res = new LocalRes(path, this);
                res.LoadAsync();
            }

            return res;
        }
    }

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