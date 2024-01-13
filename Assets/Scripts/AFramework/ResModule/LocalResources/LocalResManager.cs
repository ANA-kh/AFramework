namespace AFramework.ResModule.LocalResources
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
}