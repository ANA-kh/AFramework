using System;
using System.Collections;
using System.Collections.Generic;
using AFramework.ResModule.LocalResources;
using Object = UnityEngine.Object;

namespace AFramework.ResModule
{
    public interface IRes<TProgress, out T> : IDisposable, IProgressResult<TProgress, T>
    {
        void Retain();
        void Retain(object owner);

        void Release();
        //void IsUnUsed();
        //IEnumerable<IRes> GetDependencies();
    }

    public interface IResManager
    {
        IResLoader GetLoader();
        void UnUseRes(Res res);
    }

    // public interface IResLoader
    // {
    //     IEnumerator LoadAsync(ProgressResult<float, Object> loadResult, string path);
    //     void Unload(Object result);
    // }

    public interface IResLoader //: IResLoader where T : Object
    {
        new IEnumerator LoadAsync(IProgressPromise<float, Object> loadResult, string path, System.Type type);
        new void Unload(Object result);
        Object Load(string path, System.Type type);
    }
}