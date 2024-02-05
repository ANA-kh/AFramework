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
            BuildBundleInfos = new Dictionary<string, BuildBundleInfo>(); //bundleName->BuildBundleInfo
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

        public void CacheBundleInfos() { }
    }
}