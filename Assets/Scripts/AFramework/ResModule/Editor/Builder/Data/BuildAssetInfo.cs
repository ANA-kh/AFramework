using System.Collections.Generic;
using UnityEditor;

namespace AFramework.ResModule.Editor.Builder
{
    public class BuildAssetInfo
    {
        public BuildAssetInfo(string assetPath)
        {
            AssetPath = assetPath;
            AssetGUID = AssetDatabase.AssetPathToGUID(assetPath);
            AssetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            DependBundleNames = new HashSet<string>();
        }

        public BuildAssetInfo(string assetPath, string dependBundleName)
        {
            AssetPath = assetPath;
            AssetGUID = AssetDatabase.AssetPathToGUID(assetPath);
            AssetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            DependBundleNames = new HashSet<string> { dependBundleName };
        }

        public BuildAssetInfo(string assetPath, string dependBundleName, bool encrypt) : this(assetPath,
            dependBundleName)
        {
            Encrypt = encrypt;
        }

        public string AssetPath;
        public string AssetGUID;
        public System.Type AssetType;
        /// <summary>
        /// 包名,决定了打进那个包
        /// </summary>
        public string BundleName;
        public bool Encrypt;
        public List<BuildAssetInfo> DependAssets;
        public HashSet<string> DependBundleNames = new HashSet<string>();
    }
}