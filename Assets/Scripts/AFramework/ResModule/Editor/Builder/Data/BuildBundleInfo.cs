using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AFramework.ResModule.Editor.Builder
{
    [Serializable]
    public class BuildBundleInfo : ISerializationCallbackReceiver
    {
        public string BundleName;
        public uint UnityCRC;
        public string UnityHash;
        public string FileHash;
        public string FileCRC;
        public long FileSize;
        public bool Encrypt;
        public HashSet<string> DependBundleNames = new HashSet<string>();
        private HashSet<BuildAssetInfo> _buildAssetInfos = new HashSet<BuildAssetInfo>();
        [SerializeField]
        private string[] _dependBundleNames;
        [SerializeField]
        private string[] _assetNames;

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

        public void OnBeforeSerialize()
        {
            _dependBundleNames = DependBundleNames.ToArray();
            _assetNames = _buildAssetInfos.Select(info => info.AssetPath).ToArray();
        }

        public void OnAfterDeserialize() { }
    }
}