namespace AFramework.ResModule.LocalResources
{
    public class LocalResManager : ResManager
    {
        protected override Res GetOrCreateRes(string path)
        {
            var key = path;
            if (!_resMap.TryGetValue(key, out var res))
            {
                res = new LocalRes(path, this);
                Retain(res);
            }

            return res;
        }
    }
}