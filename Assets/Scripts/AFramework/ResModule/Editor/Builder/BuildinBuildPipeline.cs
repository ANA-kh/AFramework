using System;
using AFramework.ResModule.BundleResources;
using AFramework.ResModule.Editor.Builder.BuildContext;
using AFramework.ResModule.Setting;
using AFramework.ResModule.Utilities;
using UnityEditor;
using UnityEngine;
using FileUtil = AFramework.ResModule.Utilities.FileUtil;

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
            //TODO 加密
            Build_GenerateManifest(buildContext);
            //TODO 报告
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

        private void Build_GenerateManifest(BuildContext.BuildContext buildContext)
        {
            var buildMap = buildContext.GetContextObject<BuildMap>();
            var buildParameter = buildContext.GetContextObject<BuildParameter>();
            //补全buildBundleInfo信息.  CRC, FileHash, FileSize, Depends
            var unityManifest = buildContext.GetContextObject<AssetBundleManifest>();
            foreach (var bundleInfo in buildMap.BuildBundleInfos.Values)
            {
                var filePath = PathUtility.CombinePaths(buildParameter.BuildOutputCachePath, bundleInfo.BundleName);
                BuildPipeline.GetCRCForAssetBundle(filePath, out var UnityCRC);
                bundleInfo.UnityCRC = UnityCRC;
                bundleInfo.FileHash = HashUtility.FileCRC32(filePath);
                bundleInfo.FileCRC = HashUtility.FileCRC32(filePath);
                bundleInfo.FileSize = FileUtil.GetFileSize(filePath);
            }

            buildMap.GenBundleDepends(unityManifest);

            //生成manifest
            var manifest = new Manifest();
            manifest.AppVersion = buildParameter.AppVersion;
            manifest.ResVersion = buildParameter.ResVersion;
            buildMap.GetBuildInfo(out manifest.BundleInfos, out manifest.AssetInfos);

            //保存manifest为文件
            var outputDir = buildParameter.BuildOutputCachePath;
            var manifestPath = PathUtility.CombinePaths(outputDir, BundleSetting.ManifestFilename);
            string json = JsonUtility.ToJson(manifest, true);
            FileUtil.WriteAllText(manifestPath, json);
            Debug.Log($"Generate manifest success : {manifestPath}");

            //创建清单hash文件
            var manifestHashPath =
                PathUtility.CombinePaths(outputDir, $"{BuildSetting.ManifestFileName}_{manifest.AppVersion}_{manifest.ResVersion}.hash");
            var hash = HashUtility.FileMD5(manifestPath);
            FileUtil.WriteAllText(manifestHashPath, hash);
        }

        private void Build_CopyToOutput(BuildContext.BuildContext buildContext)
        {
            //copy to destDir
            var buildParameter = buildContext.GetContextObject<BuildParameter>();
            var sourcePath = buildParameter.BuildOutputCachePath;
            var destPath = buildParameter.CopyOutputPath;
            FileUtil.CopyDirectory(sourcePath, destPath);

            //copy to streamingAssets  TODO 那些要copy到streamingAssets的文件  独立为单独流程
            CopyToStreamingAssets();
        }

        private void Build_Post(BuildContext.BuildContext buildContext) { }

        #endregion

        [MenuItem("AFramework/CopyToStreamingAssets")]
        public static void CopyToStreamingAssets()
        {
            var buildSo = BuildSO.GetDefaultBuildSo();
            var buildParameter = buildSo.BuildParameter;
            var sourcePath = buildParameter.CopyOutputPath;
            var destPath = BundleSetting.StreamingAssetsRoot;
            FileUtil.CopyDirectory(sourcePath, destPath);
        }
    }
}