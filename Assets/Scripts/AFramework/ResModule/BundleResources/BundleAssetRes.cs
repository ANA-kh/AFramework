using System;
using System.Collections;
using UnityEngine;

namespace AFramework.ResModule.BundleResources
{
    
    public class BundleAssetRes:Res
    {
        public BundleAssetRes(string path, IResManager resManager,BundleRes bundleRes) : base(path, resManager)
        {
            OwnerBundleRes = bundleRes;
            OwnerBundleRes.Retain();
        }
        
        protected BundleRes OwnerBundleRes;
        public override IRes Load()
        {
            OwnerBundleRes.Load();
            _result = OwnerBundleRes.LoadAsset(_path);
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

            var assetLoadDown = false;
            OwnerBundleRes.LoadAssetAsync(_path, result =>
            {
                assetLoadDown = true;
                _result = result;
                OnFinish();
            });
            
            while (!assetLoadDown)
            {
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
}