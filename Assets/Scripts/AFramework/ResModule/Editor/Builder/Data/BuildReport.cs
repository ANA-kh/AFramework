using System;
using System.Collections.Generic;

namespace AFramework.ResModule.Editor.Builder
{
    [Serializable]
    public class BuildReport
    {
        public int AppVersion;
        public int ResVersion;
        public List<BuildAssetInfo> AssetInfos;
        public List<BuildBundleInfo> BundleInfos;
    }
}