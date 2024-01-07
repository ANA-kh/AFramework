using System;
using UnityEngine;

namespace AFramework.ResModule
{
    public class ProgressResult<TProgress, TResult> : IProgressResult<TProgress, TResult>, IProgressPromise<TProgress, TResult>
    {
        protected TProgress _progress;
        protected object _result;
        protected Exception _exceptipn;
        protected bool _done;
        private Action<TProgress> progressCallback;
        private Action<IProgressResult<TProgress, TResult>> _callback;
        private Action<TProgress> _progressCallback;

        public TResult Result => _result != null ? (TResult)_result : default(TResult);
        public TProgress Progress => _progress;
        public Exception Exception => _exceptipn;
        /// <summary>
        /// 完成或异常
        /// </summary>
        public bool IsDone => _done;
        public void Retain()
        {
            throw new NotImplementedException();
        }

        public void Retain(object owner)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }

        public void OnCallback(Action<IProgressResult<TProgress, TResult>> callback)
        {
            if (callback == null)
                return;
            if (_done)
            {
                try
                {
                    callback(this);
                }
                catch (Exception e)
                {
                    Debug.Log($"Class[{this.GetType()}] callback exception.Error:{e}");
                }
                return;
            }
            _callback += callback;
        }

        public void OnProgressCallback(Action<TProgress> callback)
        {
            if (callback == null)
                return;
            if (_done)
            {
                try
                {
                    callback(_progress);
                }
                catch (Exception e)
                {
                    Debug.Log($"Class[{this.GetType()}] callback exception.Error:{e}");
                }
                return;
            }
            _progressCallback += callback;
        }

        public void SetResult(TResult result)
        {
            if (_done)
                return;
            _result = result;
            _done = true;
            RaiseOnCallback();
        }
        
        public void SetException(Exception exception)
        {
            if (_done)
                return;
            _exceptipn = exception;
            _done = true;
            RaiseOnCallback();
        }
        
        public void UpdateProgress(TProgress progress)
        {
            _progress = progress;
            RaiseOnProgressCallback(progress);
        }
        
        private void RaiseOnCallback()
        {
            if (_callback == null)
                return;
            try
            {
                var list = _callback.GetInvocationList();
                _callback = null;
                foreach (Action<IProgressResult<TProgress, TResult>> cb in list)
                {
                    try
                    {
                        cb(this);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _progressCallback = null;
            }
        }
        
        private void RaiseOnProgressCallback(TProgress progress)
        {
            if (_progressCallback == null)
                return;
            try
            {
                var list = _progressCallback.GetInvocationList();
                _progressCallback = null;
                foreach (Action<TProgress> cb in list)
                {
                    try
                    {
                        cb(progress);
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Class[{this.GetType()}] callback exception.Error:{e}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Class[{this.GetType()}] callback exception.Error:{e}");
            }
        }
    }
}