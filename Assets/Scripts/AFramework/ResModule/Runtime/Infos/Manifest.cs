using System;
using System.Collections.Generic;

namespace AFramework.ResModule
{
    public class BundleInfo
    {
        public string BundleName;
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
    }
}