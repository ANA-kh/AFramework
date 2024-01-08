using System;

namespace AFramework.ResModule
{
    public interface IPromise<in TResult>
    {
        void SetResult(TResult result);
        void SetException(Exception exception);
    }
    
    public interface IProgressPromise<TProgress, TResult> :IPromise<TResult>
    {
        void UpdateProgress(TProgress progress);
    }
}