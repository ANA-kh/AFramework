using System.Collections.Generic;
using AFramework.ResModule.Utilities;
#if UNITY_ANDROID && !UNITY_EDITOR
using Ionic.Zip;
using System.Text.RegularExpressions;
#endif

namespace AFramework.ResModule.BundleResources
{
    public class BundleManager : ResManager
    {
        private BundleManifest _bundleManifest;
        
        public BundleManager(BundleManifest bundleManifest)
        {
            _bundleManifest = bundleManifest;
        }

        protected override Res GetOrCreateRes(string path)
        {
            if (!_resMap.TryGetValue(path, out var res))
            {
                BundleInfo bundleInfo = _bundleManifest.GetBundleInfoByAssetPath(path);
                res = new BundleAssetRes(path, this, GetOrCreateBundleRes(bundleInfo));
                Retain(res);
            }

            return res;
        }

        private BundleRes GetOrCreateBundleRes(BundleInfo bundleInfo)
        {
            //!!! 用BundleName作为Key,要求asset的path不能和bundle的名字一样
            if (!_resMap.TryGetValue(bundleInfo.Name, out var res))
            {
                res = new BundleRes(bundleInfo.Name, this, bundleInfo);
                Retain(res);
            }

            return res as BundleRes;
        }
        
        private FileBundleLoader _fileBundleLoader = new FileBundleLoader();

        public IBundleLoader GetBundleLoader(BundleInfo bundleInfo)
        {
            return _fileBundleLoader;
        }
        
        private IBundleLoader CreateBundleLoader(BundleInfo bundleInfo)
        {
            return _fileBundleLoader;
        }

        public List<BundleRes> GetDependencies(BundleInfo bundleInfo)
        {
            BundleInfo[] dependenciesInfo = _bundleManifest.GetDependencies(bundleInfo);
            List<BundleRes> dependencies = new List<BundleRes>();
            foreach (var info in dependenciesInfo)
            {
                dependencies.Add(GetOrCreateBundleRes(info));
            }

            return dependencies;
        }
    }
}