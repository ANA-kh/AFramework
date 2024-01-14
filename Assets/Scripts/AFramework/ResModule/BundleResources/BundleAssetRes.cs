using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AFramework.ResModule.BundleResources
{
    
    public class BundleAssetRes:Res
    {
        public BundleAssetRes(string path, IResManager resManager,BundleRes bundleRes) : base(path, resManager)
        {
            OwnerBundleRes = bundleRes;
        }
        
        protected BundleRes OwnerBundleRes;
        public override IRes Load()
        {
            OwnerBundleRes.Load();
            _result = OwnerBundleRes.LoadAsset();
            return this;
        }

        protected override IEnumerator CoLoad()
        {
            OwnerBundleRes.LoadAsync();
            while (!OwnerBundleRes.IsDone)
            {
                _progressCallback?.Invoke(OwnerBundleRes.Progress/2);
                yield return null;
            }
            OwnerBundleRes.LoadAssetAsync(result =>
            {
                _result = result;
                OnFinish();
            });
            
            while (!OwnerBundleRes.IsDone)
            {
                _progressCallback?.Invoke(0.5f + OwnerBundleRes.Progress/2);
                yield return null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            OwnerBundleRes.Release();
            base.Dispose(disposing);
        }
    }

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
            throw new System.NotImplementedException();
        }

        protected override IEnumerator CoLoad()
        {
            IBundleLoader loader = BundleManager.GetBundleLoader(BundleInfo);
            loader.LoadBundle(result =>
            {
                _assetBundle = result;
            });
            
            List<BundleRes> denpendencies = BundleManager.GetDependencies(BundleInfo);
            if (denpendencies.Count <= 0) yield break;
            
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
                Debug.LogException(new Exception($"Load asset bundle failure.The asset bundle named \"{BundleInfo.Name}\" is not found."));
            
            OnFinish();
        }

        public Object LoadAsset()
        {
            throw new System.NotImplementedException();
        }

        public void LoadAssetAsync(Action<Object> onFinish)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IBundleLoader
    {
        void LoadBundle(Action<AssetBundle> action);
    }
}