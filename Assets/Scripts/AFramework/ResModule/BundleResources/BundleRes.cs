using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AFramework.ResModule.BundleResources
{
    public class BundleRes : Res
    {
        protected BundleInfo BundleInfo;
        protected BundleManager BundleManager;
        private AssetBundle _assetBundle;
        public override Object Result => _assetBundle;
        
        public BundleRes(string path, BundleManager resManager, BundleInfo bundleInfo) : base(path, resManager)
        {
            BundleManager = resManager;
            BundleInfo = bundleInfo;
        }
        public override IRes Load()
        {
            IBundleLoader loader = BundleManager.GetBundleLoader(BundleInfo);
            _assetBundle = loader.LoadBundle(BundleInfo);
            _result = _assetBundle;
            List<BundleRes> denpendencies = BundleManager.GetDependencies(BundleInfo);
            foreach (var res in denpendencies)
            {
                res.Load();
            }
            OnFinish();
            return this;
        }

        protected override IEnumerator CoLoad()
        {
            IBundleLoader loader = BundleManager.GetBundleLoader(BundleInfo);
            var selfLoadDown = false;
            loader.LoadBundleAsync(BundleInfo,result =>
            {
                _assetBundle = result;
                _result = result;
                selfLoadDown = true;
            });
            
            List<BundleRes> denpendencies = BundleManager.GetDependencies(BundleInfo);
            if (denpendencies.Count > 0)
            {
                //异步加载时,避免依赖资源和依赖资源的依赖资源都在同一帧开协程
                yield return null;
                foreach (var res in denpendencies)
                {
                    res.LoadAsync();
                }

                var finished = false;
                var progress = 0f;
                while (!finished)
                {
                    yield return null;
                    finished = true;
                    progress = 0;
                    foreach (var res in denpendencies)
                    {
                        if (!res.IsDone) finished = false;
                        progress += res.Progress;
                    }

                    _progressCallback?.Invoke(progress / (denpendencies.Count + 1));
                }

                //如果有依赖加载失败,并不会设置自己也加载失败.
                if (_assetBundle == null)
                    Debug.LogException(new Exception(
                        $"Load asset bundle failure.The asset bundle named \"{BundleInfo.Name}\" is not found."));

                
            }
           
            while (!selfLoadDown)
            {
                yield return null;
            }
            OnFinish();
        }

        public Object LoadAsset(string path)
        {
            Check();
            if(string.IsNullOrEmpty(path))
                throw new Exception("path is null or empty");
            
            var fullName = GetFullName(path);
            return _assetBundle.LoadAsset(fullName);
        }

        public void LoadAssetAsync(string path, Action<Object> onFinish)
        {
            try
            {
                Check();
                if(string.IsNullOrEmpty(path))
                    throw new Exception("path is null or empty");
                
                AssetBundleRequest request;
                var fullName = GetFullName(path);
                request = _assetBundle.LoadAssetAsync(fullName);
                request.completed += operation =>
                {
                    onFinish?.Invoke(request.asset);
                };
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                onFinish?.Invoke(null);
            }
        }
        protected  void Check()
        {
            if (this._disposed)
                throw new System.ObjectDisposedException(this._path);

            if (!this.IsDone || this._assetBundle == null)
                throw new System.Exception(string.Format("The AssetBundle '{0}' isn't ready.", this._path));
        }
        private const string ASSETS = "Assets/";
        protected  string GetFullName(string name)
        {   
            //TODO 删除 
            if (name.StartsWith(ASSETS, System.StringComparison.OrdinalIgnoreCase) || name.IndexOf("/") < 0)
                return name;
            return string.Format("{0}{1}", ASSETS, name);
        }
    }
}