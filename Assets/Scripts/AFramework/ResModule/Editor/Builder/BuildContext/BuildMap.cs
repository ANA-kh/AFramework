using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace AFramework.ResModule.Editor.Builder.BuildContext
{
    public class BuildMap
    {
        public Dictionary<string, BuildBundleInfo>
            BuildBundleInfos = new Dictionary<string, BuildBundleInfo>(); //bundleName->BuildBundleInfo]]]]]]]]]]]]]]]]]]]]]
        private readonly List<BuildFilter> _buildFilters;

        public BuildMap(List<BuildFilter> buildFilters)
        {
            _buildFilters = buildFilters.Where(x => x.Active).ToList();
        }

        public AssetBundleBuild[] GetAssetBundleBuilds()
        {
            List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
            foreach (var buildBundleInfo in BuildBundleInfos.Values)
            {
                AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
                assetBundleBuild.assetBundleName = buildBundleInfo.BundleName;
                assetBundleBuild.assetNames = buildBundleInfo.GetAssetNames();
                assetBundleBuilds.Add(assetBundleBuild);
            }

            return assetBundleBuilds.ToArray();
        }

        public void CollectAll()
        {
            Dictionary<string, BuildAssetInfo> mainAssets = new Dictionary<string, BuildAssetInfo>();
            Dictionary<string, BuildAssetInfo> dependAssets = new Dictionary<string, BuildAssetInfo>();
            //1.收集所有资源  主要资源按规则打包.   
            foreach (var buildFilter in _buildFilters)
            {
                if (!buildFilter.Active)
                    continue;
                var buildAssetInfos = buildFilter.GetBuildAssetInfos();
                if (buildAssetInfos == null)
                    continue;

                //收集主资源
                mainAssets = buildAssetInfos.ToDictionary(info => info.AssetGUID, info => info);
                //收集依赖资源,并合并依赖包名
                foreach (var buildAssetInfo in buildAssetInfos)
                {
                    foreach (var dependAsset in buildAssetInfo.DependAssets)
                    {
                        if (mainAssets.ContainsKey(dependAsset.AssetGUID))
                            continue;

                        if (dependAssets.TryGetValue(dependAsset.AssetGUID, out var existDependAsset))
                        {
                            existDependAsset.DependBundleNames.UnionWith(buildAssetInfo.DependBundleNames);
                        }
                        else
                        {
                            dependAssets.Add(dependAsset.AssetGUID, dependAsset);
                        }
                    }
                }
            }

            //2.依赖资源自动打包(规则:若只被单个AssetBundle依赖,则打包到该AssetBundle中. 否则单独打包)
            foreach (var dependAssetInfo in dependAssets.Values)
            {
                if (dependAssetInfo.DependBundleNames.Count == 1)
                {
                    var dependBundleName = dependAssetInfo.DependBundleNames.First();
                    dependAssetInfo.BundleName = dependBundleName;
                }
                else
                {
                    dependAssetInfo.BundleName = GetSharedBundleName(dependAssetInfo.AssetPath);
                }
            }

            //3.生成BuildBundleInfo
            Assert.NotNull(mainAssets);
            foreach (var buildAssetInfo in mainAssets.Values)
            {
                PackAsset(buildAssetInfo);
            }

            foreach (var dependAssetInfo in dependAssets.Values)
            {
                PackAsset(dependAssetInfo);
            }

            //4.检查资源名长度
            foreach (var buildBundleInfo in BuildBundleInfos.Values)
            {
                var fileName = buildBundleInfo.BundleName;
                if (fileName.Length > 260)
                {
                    throw new Exception($"BundleName is too long : {fileName}");
                }
            }
        }

        private void PackAsset(BuildAssetInfo buildAssetInfo)
        {
            if (!BuildBundleInfos.TryGetValue(buildAssetInfo.BundleName, out var buildBundleInfo))
            {
                buildBundleInfo = new BuildBundleInfo(buildAssetInfo.BundleName);
                BuildBundleInfos.Add(buildAssetInfo.BundleName, buildBundleInfo);
            }

            buildBundleInfo.AddAssets(buildAssetInfo);
        }

        private string GetSharedBundleName(string assetPath)
        {
            return $"shared_{assetPath.Replace('/', '_').ToLower()}";
        }

        /// <summary>
        /// 比较生成后的manifest和自己收集的buildMap是否一致
        /// </summary>
        /// <param name="manifest"></param>
        public void CheckManifest(AssetBundleManifest manifest)
        {
            var manifestBundleNames = manifest.GetAllAssetBundles();
            var buildBundleNames = BuildBundleInfos.Keys.ToArray();
            foreach (var manifestBundleName in manifestBundleNames)
            {
                if (!buildBundleNames.Contains(manifestBundleName))
                {
                    Debug.LogWarning($"manifest has bundle not in buildMap:{manifestBundleName}");
                }
            }

            foreach (var buildBundleName in buildBundleNames)
            {
                if (!manifestBundleNames.Contains(buildBundleName))
                {
                    Debug.LogWarning($"buildMap has bundle not in manifest:{buildBundleName}");
                }
            }
        }

        public void GetBuildInfo(out List<BundleInfo> bundleInfos, out List<AssetInfo> assetInfos) //TODO manifest传进来有点怪,此函数考虑移到外面(buildTask)
        {
            bundleInfos = new List<BundleInfo>();
            assetInfos = new List<AssetInfo>();

            //使用id表示依赖,名字str太长
            var cacheBundleID = new Dictionary<string, int>();
            foreach (var value in BuildBundleInfos.Values)
            {
                var bundleInfo = new BundleInfo();
                bundleInfo.BundleName = value.BundleName;
                bundleInfo.UnityCRC = value.UnityCRC;
                bundleInfo.FileHash = value.FileHash;
                bundleInfo.FileCRC = value.FileCRC;
                bundleInfo.FileSize = value.FileSize;
                bundleInfo.Encrypted = value.Encrypt;
                bundleInfos.Add(bundleInfo);
                cacheBundleID.Add(value.BundleName, bundleInfos.Count - 1);
            }

            foreach (var value in BuildBundleInfos.Values)
            {
                var bundleInfo = bundleInfos[cacheBundleID[value.BundleName]];
                bundleInfo.DependBundleIDs = value.DependBundleNames.Select(x => cacheBundleID[x]).ToArray();
            }

            foreach (var value in BuildBundleInfos.Values)
            {
                foreach (var buildAssetInfo in value.GetMainAssets())
                {
                    var assetInfo = new AssetInfo();
                    assetInfo.AssetPath = buildAssetInfo.AssetPath;
                    assetInfo.BundleID = cacheBundleID[value.BundleName];
                    assetInfos.Add(assetInfo);
                }
            }
        }

        public void GenBundleDepends(AssetBundleManifest manifest)
        {
            foreach (var buildBundleInfo in BuildBundleInfos.Values)
            {
                var myDepends = new HashSet<string>();
                foreach (var buildAssetInfo in buildBundleInfo.GetMainAssets())
                {
                    myDepends.UnionWith(buildAssetInfo.DependBundleNames);
                }

                var unityDepends = manifest.GetDirectDependencies(buildBundleInfo.BundleName);
                var uniqueInUnityDepends = unityDepends.Except(myDepends);

                var uniqueInDependBundleNames = myDepends.Except(unityDepends);

                Debug.LogWarning("Unique in unityDepends:");
                foreach (var str in uniqueInUnityDepends)
                {
                    Debug.LogWarning(str);
                }

                Debug.LogWarning("Unique in myDepends:");
                foreach (var str in uniqueInDependBundleNames)
                {
                    Debug.LogWarning(str);
                }

                buildBundleInfo.DependBundleNames = new HashSet<string>(unityDepends);
            }
        }
    }
}