using System;

namespace AFramework.ResModule
{
    public interface IPromise<TResult>
    {
        TResult Result { get; }
        Exception Exception { get; }
        bool IsDone { get; }
        void SetResult(TResult result);
        void SetException(Exception exception);
    }
    
    public interface IProgressPromise<TProgress, TResult> :IPromise<TResult>
    {
        TProgress Progress { get; }
        void UpdateProgress(TProgress progress);
    }
}