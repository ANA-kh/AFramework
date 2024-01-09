using System;

namespace AFramework.ResModule
{
    public interface IPromise<in TResult>
    {
        void SetResult(TResult result);
        void SetException(Exception exception);
    }
    
    public interface IProgressPromise<in TResult> :IPromise<TResult>
    {
        void UpdateProgress(float progress);
    }
}