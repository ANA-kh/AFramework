using System;
using System.Collections;
using AFramework.ResModule.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AFramework.ResModule
{
    public abstract class Res:IRes
    {
        protected bool _done;
        protected Object _result;
        protected float _progress;
        protected int _refCount;
        protected Action<Object> _callback;
        protected Action<float> _progressCallback;
        public bool IsDone => _done;
        public Object Result => _result;
        public float Progress => _progress;
        
        public IResManager ResManager { get; set; }

        public Res(string path,IResManager resManager)
        {
            _path = path;
            ResManager = resManager;
        }
        
        public void Retain()
        {
            if (this._disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            this._refCount++;
            ResManager.Retain(this);
        }

        public void Retain(object owner)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            if (this._disposed)
                return;

            this._refCount--;
            if (CheckUnUsed())
            {
                ResManager.Release(this);
            }
        }

        private bool CheckUnUsed()
        {
            //TODO weakReference
            return _refCount <= 0;
        }

        public abstract IRes Load();
        public virtual IRes LoadAsync()
        {
            if (_loading || _done)
                return this;
            
            _loading = true;
            CoroutineRunner.MStartCoroutine(Wrap(CoLoad()));
            return this;
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
                _loading = false;
                Release();
            }
        }
        protected abstract IEnumerator CoLoad();
        
        public void OnCallback(Action<Object> callback)
        {
            if (callback == null)
                return;
            if (_done)
            {
                try
                {
                    callback(_result);
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
        
        #region IDisposable
        protected bool _disposed;
        protected string _path;
        private bool _loading;

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            //TODO 若加载还未完成,如何处理?  目前加载前会retain,加载完成会release,故加载过程中不会被回收.  考虑别的方式
            if (_disposed)
                return;
            
            if (disposing)
            {
                _callback = null;
                _progressCallback = null;
                _result = null;
                ResManager = null;
            }
            _disposed = true;
        }
        #endregion

        protected void OnFinish()
        {
            _done = true;
            _progressCallback?.Invoke(1);
            _callback?.Invoke(_result);
        }

        public virtual string Key()
        {
            return _path;
        }
    }
}