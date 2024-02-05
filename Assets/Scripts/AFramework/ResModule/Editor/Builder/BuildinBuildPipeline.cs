using System;
using AFramework.ResModule.Editor.Builder.BuildContext;
using AFramework.ResModule.Utilities;
using UnityEditor;
using UnityEngine;

namespace AFramework.ResModule.Editor.Builder
{
    public partial class BuildinBuildPipeline
    {
        public void DefaultBuild()
        {
            var buildContext = new BuildContext.BuildContext();
            var buildSo = BuildSO.GetDefaultBuildSo();
            buildContext.SetContextObject(buildSo.BuildParameter);
            buildContext.SetContextObject(new BuildMap(buildSo.BuildFilters));
            Build(buildContext);
        }

        public void Build(BuildContext.BuildContext buildContext)
        {
            Build_Pre(buildContext);
            Build_CreateMap(buildContext);
            Building(buildContext);
            Build_Verify(buildContext);
            Build_GenerateManifest(buildContext);
            Build_CopyToOutput(buildContext);
            Build_Post(buildContext);
        }

        #region BuildPipeline

        private void Build_Pre(BuildContext.BuildContext buildContext)
        {
            var buildParameter = buildContext.GetContextObject<BuildParameter>();
            buildParameter.CheckBuildParameters();
        }

        private void Build_CreateMap(BuildContext.BuildContext buildContext)
        {
            var buildMap = buildContext.GetContextObject<BuildMap>();
            buildMap.CollectAll();
        }

        private void Building(BuildContext.BuildContext buildContext)
        {
            BuildParameter buildParameter = buildContext.GetContextObject<BuildParameter>();
            BuildMap buildMap = buildContext.GetContextObject<BuildMap>();
            var assetbundleBuilds = buildMap.GetAssetBundleBuilds();
            var buildResult = BuildPipeline.BuildAssetBundles(buildParameter.BuildOutputCachePath, assetbundleBuilds,
                buildParameter.GetBuildOptions(), buildParameter.BuildTarget);
            if (buildResult == null)
            {
                throw new Exception("BuildPipeline.BuildAssetBundles failed");
            }

            Debug.Log("BuildPipeline.BuildAssetBundles success");
            buildContext.SetContextObject(buildResult);
        }

        private void Build_Verify(BuildContext.BuildContext buildContext)
        {
            var buildMap = buildContext.GetContextObject<BuildMap>();
            var manifest = buildContext.GetContextObject<AssetBundleManifest>();
            buildMap.CheckManifest(manifest);
        }

        // private void Build_Encrypt(BuildContext.BuildContext buildContext)
        // {
        //     var buildParameter = buildContext.GetContextObject<BuildParameter>();
        //     var buildMap = buildContext.GetContextObject<BuildMap>();
        //     
        // }

        private void Build_GenerateManifest(BuildContext.BuildContext buildContext)
        {
            var buildMap = buildContext.GetContextObject<BuildMap>();
            var manifest = buildContext.GetContextObject<AssetBundleManifest>();
            var buildParameter = buildContext.GetContextObject<BuildParameter>();
            foreach (var bundleInfo in buildMap.BuildBundleInfos.Values)
            {
                var filePath = PathUtility.CombinePaths(buildParameter.BuildOutputCachePath, bundleInfo.BundleName);
                bundleInfo.UnityHash = manifest.GetAssetBundleHash(bundleInfo.BundleName).ToString();
                BuildPipeline.GetCRCForAssetBundle(filePath, out var UnityCRC);
                bundleInfo.UnityCRC = UnityCRC;
                bundleInfo.FileHash = HashUtility.FileCRC32(filePath);
                bundleInfo.FileCRC = HashUtility.FileCRC32(filePath);
                bundleInfo.FileSize = FileUtil.GetFileSize(filePath);
            }
        }

        private void Build_CopyToOutput(BuildContext.BuildContext buildContext) { }

        private void Build_Post(BuildContext.BuildContext buildContext) { }

        #endregion
    }
}