using System;
using System.Collections.Generic;
using AFramework.ResModule.Utilities;
using UnityEngine;

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
                res = new BundleAssetRes(path, this, GetOrCreateBundleRes(path));
                Retain(res);
            }

            return res;
        }

        // private string GetBundleKey(BundleInfo bundleInfo)
        // {
        //     return "bundle_" + bundleInfo.BundleName;
        // }

        private BundleRes GetOrCreateBundleRes(string path)
        {
            BundleInfo bundleInfo = _bundleManifest.GetBundleInfoByAssetPath(path);

            return GetOrCreateBundleRes(bundleInfo);
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

        public IBundleLoader GetBundleLoader(BundleInfo bundleInfo)
        {
            throw new NotImplementedException();
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

    [Serializable]
    public class BundleInfo
    {
        //TODO 检查这些字段的意义
        [SerializeField]
        private string name;
        [SerializeField]
        private string variant;
        [SerializeField]
        private string hash;
        [SerializeField]
        private uint crc;
        [SerializeField]
        private long fileSize;
        [SerializeField]
        private string filename;
        [SerializeField]
        private string encoding;
        [SerializeField]
        private bool published;
        [SerializeField]
        private string[] dependencies = null;
        [SerializeField]
        private string[] assets = null;
        [SerializeField]
        private bool streamedScene;

        public string Name => name;
        public string[] Assets => assets;
        public bool Published => published;
        public string[] Dependencies => dependencies;
    }
}