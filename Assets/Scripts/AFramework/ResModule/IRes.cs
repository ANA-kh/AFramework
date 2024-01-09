using System.Collections.Generic;

namespace AFramework.ResModule
{
    public interface IRes
    {
        object Result { get; }
        void Retain();
        void Retain(object owner);
        void Release();
        //void IsUnUsed();
        //IEnumerable<IRes> GetDependencies();
        
        ProgressResult<float,IRes> ProgressResult { get; }
    }
    
    public interface IResManager
    {
        IResLoader GetLoader<T>(string url) where T : IResLoader, new();
        IResLoader NewLoader<T>(string url) where T : IResLoader, new();
    }
    
    public interface IResLoader
    {
        float Progress { get; }
    }
}