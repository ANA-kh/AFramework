using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AFramework.ResModule.Editor.Builder
{
    public class BuildBundleInfo
    {
        private HashSet<BuildAssetInfo> _buildAssetInfos = new HashSet<BuildAssetInfo>();
        public readonly string BundleName;
        public bool Encrypt;
        public HashSet<string> DependBundleNames = new HashSet<string>();

        public BuildBundleInfo(string bundleName)
        {
            BundleName = bundleName;
        }

        public uint UnityCRC { get; set; }
        public string FileHash { get; set; }
        public string FileCRC { get; set; }
        public long FileSize { get; set; }

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
            else
            {
                if (buildAssetInfo.Encrypt)
                {
                    Encrypt = true;
                }
            }
        }

        public string[] GetAssetNames()
        {
            return _buildAssetInfos.Select(info => info.AssetPath).ToArray();
        }

        public List<BuildAssetInfo> GetMainAssets()
        {
            return _buildAssetInfos.Where(x => x.IsMainAsset).ToList();
        }
    }
}