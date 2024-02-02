#if UNITY_EDITOR
namespace AFramework.ResModule.SimulationResources
{
    public class SimulationResManager : ResManager
    {
        protected override Res GetOrCreateRes(string path)
        {
            if (_resMap.TryGetValue(path, out var res))
                return res;
            res = new SimulationRes(path, this);
            Retain(res);
            return res;
        }
    }
}
#endif