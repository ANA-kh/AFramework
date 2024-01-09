using System;

namespace AFramework.ResModule
{
    public interface IProgressResult<TProgress, TResult>
    {
        TProgress Progress { get; }
        TResult Result { get; }
        Exception Exception { get; }
        bool IsDone { get; }
        
        void Retain();
        void Retain(object owner);
        void Release();
        
        /// <summary>
        /// Called when the task is finished.
        /// </summary>
        /// <param name="callback"></param>
        void OnCallback(Action<IProgressResult<TProgress, TResult>> callback);

        /// <summary>
        /// Called when the progress update.
        /// </summary>
        /// <param name="callback"></param>
        void OnProgressCallback(Action<TProgress> callback);
        void UpdateProgress(TProgress progress);
    }
}