using System;
using System.Collections;
using AFramework.ResModule.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AFramework.ResModule.LocalResources
{
    public class Res : IRes<float, Object>, IProgressPromise<float, Object>
    {
        private string _path;
        private int _refCount = 0;
        private object _lock = new object();
        private IResLoader _loader;
        private IResManager _manager;

        protected float _progress;
        protected Object _result;
        protected Exception _exceptipn;
        protected bool _done;
        private Action<IProgressResult<float, Object>> _callback;
        private Action<float> _progressCallback;

        public Object Result => _result != null ? (Object)_result : default(Object);
        public float Progress => _progress;
        public Exception Exception => _exceptipn;
        public bool IsDone => _done;
        public string Path => _path;

        public Res(string path, IResManager manager)
        {
            _path = path;
            _manager = manager;
            _loader = manager.GetLoader();
        }

        public void Load()
        {
            UpdateProgress(1f);
            SetResult(_loader.Load(_path, typeof(Object)));
        }

        public void LoadAsync()
        {
            CoroutineRunner.MStartCoroutine(Wrap(_loader.LoadAsync(this, _path, typeof(Object))));
        }

        private IEnumerator Wrap(IEnumerator enumerator)
        {
            Retain();
            try
            {
                yield return enumerator;
            }
            finally
            {
                Release();
            }
        }

        public void UpdateProgress(float progress)
        {
            _progress = progress;
            RaiseOnProgressCallback(progress);
        }

        public void SetResult(Object result)
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

        public void OnCallback(Action<IProgressResult<float, Object>> callback)
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

        public void OnProgressCallback(Action<float> callback)
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

        private void RaiseOnCallback()
        {
            if (_callback == null)
                return;
            try
            {
                var list = _callback.GetInvocationList();
                _callback = null;
                foreach (Action<IProgressResult<float, Object>> cb in list)
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

        private void RaiseOnProgressCallback(float progress)
        {
            if (_progressCallback == null)
                return;
            try
            {
                var list = _progressCallback.GetInvocationList();
                foreach (Action<float> cb in list)
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

        public void Retain()
        {
            lock (_lock)
            {
                if (this._disposed) throw new ObjectDisposedException(_path);

                _refCount++;
            }
        }

        public void Retain(object owner)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            lock (_lock)
            {
                if (this._disposed) return;

                _refCount--;
                CheckUnUse();
            }
        }

        private void CheckUnUse()
        {
            if (_refCount <= 0)
                _manager.UnUseRes(this);
        }

        #region IDisposable

        private bool _disposed = false;

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                }

                // 释放非托管资源
                _loader.Unload(_result);
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        #endregion
    }
}