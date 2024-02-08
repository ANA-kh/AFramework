using System;
using System.Collections.Generic;

namespace AFramework.ResModule
{
    public class BundleInfo
    {
        public string BundleName; //带后缀的全名
        public uint UnityCRC;
        public string FileHash;
        public string FileCRC;
        public long FileSize;
        public bool Encrypted;
        public int[] DependBundleIDs;
    }

    public class AssetInfo
    {
        public string AssetPath;
        public int BundleID;
    }

    [Serializable]
    public class Manifest
    {
        public int AppVersion;
        public int ResVersion;
        public List<BundleInfo> BundleInfos;
        public List<AssetInfo> AssetInfos;

        public Dictionary<string, AssetInfo> AssetInfoMap;

        public void Initialize()
        {
            AssetInfoMap = new Dictionary<string, AssetInfo>();
            foreach (var assetInfo in AssetInfos)
            {
                AssetInfoMap[assetInfo.AssetPath] = assetInfo;
            }
        }

        public BundleInfo GetBundleInfoByAssetPath(string path)
        {
            var assetInfo = AssetInfoMap[path];
            return BundleInfos[assetInfo.BundleID];
        }

        public BundleInfo[] GetDependencies(BundleInfo bundleInfo)
        {
            List<BundleInfo> dependencies = new List<BundleInfo>();
            foreach (var id in bundleInfo.DependBundleIDs)
            {
                dependencies.Add(BundleInfos[id]);
            }

            return dependencies.ToArray();
        }
    }
}