using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AFramework.ResModule.Utilities;
using UnityEngine;

namespace AFramework.ResModule.BundleResources
{
    [Serializable]
    public class BundleManifest : ISerializationCallbackReceiver
    {
        [SerializeField]
        private BundleInfo[] bundleInfos = null;
        [SerializeField]
        private string version;

        [NonSerialized]
        private Dictionary<string, BundleInfo> assetPath_BundleInfo;
        [NonSerialized]
        private Dictionary<string, BundleInfo> bundles = new Dictionary<string, BundleInfo>();
        
        public BundleManifest(List<BundleInfo> bundleInfos, string version)
        {
            if (bundleInfos != null)
                this.bundleInfos = bundleInfos.ToArray();
            else
                this.bundleInfos = new BundleInfo[0];

            this.version = version;
        }
        

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
        
        public virtual BundleInfo[] GetDependencies(string bundleName, bool recursive)
        {
            BundleInfo info = this.GetBundleInfo(bundleName);
            if (info == null)
                return new BundleInfo[0];

            List<BundleInfo> list = new List<BundleInfo>();
            this.GetDependencies(info, info, recursive, list);
            return list.ToArray();
        }

        protected virtual void GetDependencies(BundleInfo root, BundleInfo info, bool recursive, List<BundleInfo> list)
        {
            string[] dependencyNames = info.Dependencies;
            if (dependencyNames == null || dependencyNames.Length <= 0)
                return;

            BundleInfo[] dependencies = this.GetBundleInfos(dependencyNames);
            for (int i = 0; i < dependencies.Length; i++)
            {
                var dependency = dependencies[i];
                if (dependency.Equals(root))
                {
                    Debug.LogWarning(string.Format("It has an loop reference between '{0}' and '{1}',it is recommended to redistribute their assets.", root.Name, info.Name));
                    continue;
                    //throw new LoopingReferenceException(string.Format("There is a error occurred.It has an unresolvable loop reference between '{0}' and '{1}'.", root.Name, info.Name));
                }

                if (list.Contains(dependency))
                    continue;

                list.Add(dependency);

                if (recursive)
                    this.GetDependencies(root, dependency, recursive, list);
            }
        }
        
        public virtual BundleInfo[] GetBundleInfos(params string[] bundleNames)
        {
            if (bundleNames == null || bundleNames.Length <= 0)
                return new BundleInfo[0];

            List<BundleInfo> list = new List<BundleInfo>();
            for (int i = 0; i < bundleNames.Length; i++)
            {
                var name = Path.GetFilePathWithoutExtension(bundleNames[i]);
                BundleInfo info;
                if (bundles.TryGetValue(name, out info))
                {
                    if (info != null && !list.Contains(info))
                        list.Add(info);
                }
            }
            return list.ToArray();
        }
        
        private BundleInfo GetBundleInfo(string bundleName)
        {
            if (bundles.TryGetValue(bundleName, out var bundleInfo))
                return bundleInfo;
            
            return null;
        }
        
        public virtual BundleInfo[] GetAll()
        {
            return this.bundleInfos;
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
        
        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        
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
        private string encoding;            //编码方式
        [SerializeField]
        private bool published;
        [SerializeField]
        private string[] dependencies = null;
        [SerializeField]
        private string[] assets = null;
        [SerializeField]
        private bool streamedScene;

        public BundleInfo(string bundleName, string s, Hash128 hash128, uint u, long size, string filename1, bool b, string[] strings, string[] dependencies1, bool isStreamedScene)
        {
            name = bundleName;
            variant = s;
            hash = hash128.ToString();
            crc = u;
            fileSize = size;
            filename = filename1;
            published = b;
            assets = strings;
            dependencies = dependencies1;
            streamedScene = isStreamedScene;
        }

        public string Name => name;
        public string[] Assets => assets;
        public bool Published => published;
        public string[] Dependencies => dependencies;
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(variant))
                    return name;

                return string.Format("{0}.{1}", name, variant);
            }
        }
        public long FileSize { get => fileSize; set => fileSize = value; }
        public string Encoding { get => encoding; set => encoding = value; }
        public string Filename { get => filename; set => filename = value; }
        public string Hash { get => hash; set => hash = value; }
        public uint CRC { get => crc; set => crc = value; }
        public bool IsEncrypted { get => !string.IsNullOrEmpty(encoding); }
    }
    
}