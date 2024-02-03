using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AFramework.Editor.Builder
{
    public class BuildBundleInfo
    {
        private HashSet<BuildAssetInfo> _buildAssetInfos = new HashSet<BuildAssetInfo>();
        public readonly string BundleName;

        public BuildBundleInfo(string bundleName)
        {
            BundleName = bundleName;
        }

        public void AddAssets(BuildAssetInfo buildAssetInfo)
        {
            if (buildAssetInfo == null)
            {
                Debug.LogError("BuildAssetInfo is null");
                return;
            }

            if (!_buildAssetInfos.Add(buildAssetInfo))
            {
                Debug.LogError($"[BuildBundleInfo] BuildAssetInfo is already existed.  :{buildAssetInfo.AssetPath}");
            }
        }

        public string[] GetAssetNames()
        {
            return _buildAssetInfos.Select(info => info.AssetPath).ToArray();
        }
    }
}