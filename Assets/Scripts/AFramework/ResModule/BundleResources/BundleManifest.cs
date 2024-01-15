using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AFramework.ResModule.BundleResources
{
    [Serializable]
    public class BundleManifest : ISerializationCallbackReceiver
    {
        [SerializeField]
        private BundleInfo[] bundleInfos = null;
        [SerializeField]
        private string defaultVariant = "";
        [SerializeField]
        private string version;

        [NonSerialized]
        private Dictionary<string, BundleInfo> assetPath_BundleInfo;
        [NonSerialized]
        private Dictionary<string, BundleInfo> bundles = new Dictionary<string, BundleInfo>();

        public BundleInfo[] GetDependencies(BundleInfo bundleInfo)
        {
            if (bundleInfo.Dependencies == null || bundleInfo.Dependencies.Length == 0)
                return new BundleInfo[0];

            BundleInfo[] dependencies = new BundleInfo[bundleInfo.Dependencies.Length];
            for (int i = 0; i < bundleInfo.Dependencies.Length; i++)
            {
                dependencies[i] = GetBundleInfo(bundleInfo.Dependencies[i]);
            }

            return dependencies;
        }
        
        private BundleInfo GetBundleInfo(string bundleName)
        {
            if (bundles.TryGetValue(bundleName, out var bundleInfo))
                return bundleInfo;
            
            return null;
        }
        
        public BundleInfo GetBundleInfoByAssetPath(string path)
        {
            if (assetPath_BundleInfo == null)
            {
                Regex regex = new Regex("^assets/", RegexOptions.IgnoreCase);
                assetPath_BundleInfo = new Dictionary<string, BundleInfo>();
                foreach (var info in bundleInfos)
                {
                    if (!info.Published)
                        continue;

                    var assets = info.Assets;
                    for (int i = 0; i < assets.Length; i++)
                    {
                        var assetPath = assets[i];
                        var key = regex.Replace(assetPath, "");
                        assetPath_BundleInfo[key] = info;
                    }
                }
            }
            
            if (!assetPath_BundleInfo.TryGetValue(path, out var bundleInfo))
            {
                Debug.LogError($"BundleManifest.GetBundleInfo: {path} not found");
                return null;
            }
            
            return bundleInfo;
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize() { }
        
        public static BundleManifest Parse(string json)
        {
            return JsonUtility.FromJson<BundleManifest>(json);
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
        public string FileName => filename;
    }
}