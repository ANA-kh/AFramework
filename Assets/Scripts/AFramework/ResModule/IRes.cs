using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace AFramework.ResModule
{
    public interface IRes:IDisposable
    {
        Object Result { get; }
        /// <summary>
        /// 缓存用关键字
        /// </summary>
        /// <returns></returns>
        string Path();
        void Retain();
        void Retain(object owner);
        void Release();
        int RefCount { get; }
        
        bool IsDone { get; }
        float Progress { get; }
        //Exception Exception { get; }
        
        /// <summary>
        /// Called when the task is finished.
        /// </summary>
        /// <param name="callback"></param>
        void OnCallback(Action<Object> callback);

        /// <summary>
        /// Called when the progress update.
        /// </summary>
        /// <param name="callback"></param>
        void OnProgressCallback(Action<float> callback);
    }

    public interface IResManager
    {
        void Retain(Res res);
        void Release(Res res);
        Res Load(string path);
        Res LoadAsync(string path);
    }
    
    public interface IResLoader
    {
        float Progress { get; }
    }
}