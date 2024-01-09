using System;

namespace AFramework.ResModule
{
    public interface IProgressResult<out TResult>
    {
        float Progress { get; }
        TResult Result { get; }
        Exception Exception { get; }
        bool IsDone { get; }
        
        /// <summary>
        /// Called when the task is finished.
        /// </summary>
        /// <param name="callback"></param>
        void OnCallback(Action<IProgressResult<TResult>> callback);

        /// <summary>
        /// Called when the progress update.
        /// </summary>
        /// <param name="callback"></param>
        void OnProgressCallback(Action<float> callback);
    }
}