using System;

namespace AFramework.ResModule
{
    public class ImmutableProgressResult<TProgress, TResult> : ProgressResult<TProgress,TResult>
    {
        public ImmutableProgressResult(TProgress progress)
        {
            UpdateProgress(progress);
        }
        
        public ImmutableProgressResult(TResult result, TProgress progress)
        {
            UpdateProgress(progress);
            SetResult(result);
        }
        
        public ImmutableProgressResult(Exception exception, TProgress progress)
        {
            UpdateProgress(progress);
            SetException(exception);
        }
    }
}