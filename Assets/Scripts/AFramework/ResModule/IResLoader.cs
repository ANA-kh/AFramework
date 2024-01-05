namespace AFramework.ResModule
{
    public interface IResLoader
    {
        float Progress { get; }
        void Retain();
        void Release();
    }

    public interface IResManager
    {
        IResLoader GetLoader<T>(string url) where T: IResLoader, new();
        IResLoader NewLoader<T>(string url) where T : IResLoader, new();
    }
}