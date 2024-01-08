using System;
using System.Collections;
using AFramework.ResModule.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AFramework.ResModule
{
    public class LocalRes:Res
    {
        private string _path;

        public LocalRes(string path,IResManager resManager)
        {
            _path = path;
            ResManager = resManager;
        }

        public override string Key()
        {
            return _path;
        }
        
        private string PathWithoutExtension => Path.GetFilePathWithoutExtension(_path);

        public override Res Load()
        {
            _result = Resources.Load(PathWithoutExtension);
            OnFinish();
            return this;
        }

        protected override IEnumerator CoLoad()
        {
            var request = Resources.LoadAsync(PathWithoutExtension);
            while (!request.isDone)
            {
                _progressCallback?.Invoke(request.progress);
                yield return null;
            }
            _result = request.asset;
            if (_result == null)
            {
                Debug.LogException(new Exception($"Load asset failure.The asset named \"{PathWithoutExtension}\" is not found."));
            }
            OnFinish();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            Resources.UnloadAsset(_result);
            _result = null;
            base.Dispose(disposing);
        }
    }
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

        // public virtual IEnumerable<Res> Dependencies()
        // {
        //     return null;
        // }
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

        public abstract Res Load();
        public virtual Res LoadAsync()
        {
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

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            //TODO 若加载还未完成,如何处理?
            if (_disposed)
                return;
            
            if (disposing)
            {
                // TODO release managed resources here
                _callback = null;
                _progressCallback = null;
                _result = null;
                ResManager = null;
            }
            _disposed = true;
        }
        #endregion

        public abstract string Key();

        protected void OnFinish()
        {
            _done = true;
            _progressCallback?.Invoke(1);
            _callback?.Invoke(_result);
        }
    }
}