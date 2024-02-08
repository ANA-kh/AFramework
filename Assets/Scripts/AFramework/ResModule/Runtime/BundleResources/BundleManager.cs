using System.Collections.Generic;

namespace AFramework.ResModule.BundleResources
{
    public class BundleManager : ResManager
    {
        private Manifest _manifest;

        public BundleManager()
        {
            _manifest = BundleUtil.LoadManifest();
            _manifest.Initialize();
        }

        protected override Res GetOrCreateRes(string path)
        {
            if (!_resMap.TryGetValue(path, out var res))
            {
                BundleInfo bundleInfo = _manifest.GetBundleInfoByAssetPath(path);
                res = new BundleAssetRes(path, this, GetOrCreateBundleRes(bundleInfo));
                Retain(res);
            }

            return res;
        }

        private BundleRes GetOrCreateBundleRes(BundleInfo bundleInfo)
        {
            //!!! 用BundleName作为Key,要求asset的path不能和bundle的名字一样
            if (!_resMap.TryGetValue(bundleInfo.BundleName, out var res))
            {
                res = new BundleRes(bundleInfo.BundleName, this, bundleInfo);
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
            BundleInfo[] dependenciesInfo = _manifest.GetDependencies(bundleInfo);
            List<BundleRes> dependencies = new List<BundleRes>();
            foreach (var info in dependenciesInfo)
            {
                dependencies.Add(GetOrCreateBundleRes(info));
            }

            return dependencies;
        }
    }
}