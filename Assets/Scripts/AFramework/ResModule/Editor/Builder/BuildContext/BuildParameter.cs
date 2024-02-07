using System;
using System.IO;
using AFramework.ResModule.Setting;
using UnityEditor;
using UnityEngine;

namespace AFramework.ResModule.Editor.Builder.BuildContext
{
    public enum CompressOption
    {
        Uncompressed,
        LZ4,
        LZMA
    }

    public enum BuildMode
    {
        IncrementalBuild,
        ForceRebuild,
        SimulateBuild
    }

    [Serializable]
    public class BuildParameter
    {
        public BuildParameter() { }

        public int AppVersion;
        public int ResVersion;
        public CompressOption CompressOption = CompressOption.Uncompressed;
        public BuildMode BuildMode = BuildMode.ForceRebuild;
        public bool DisableWriteTypeTree = false; //禁止写入类型树结构（可以降低包体和内存并提高加载效率）
        public bool IgnoreTypeTreeChanges = true;

        public string BuildOutputRoot => BuildSetting.DefaultBuildOutputRoot();
        public BuildTarget BuildTarget => EditorUserBuildSettings.activeBuildTarget; //不同平台打包

        /// <summary>
        /// unity Build输出目录
        /// unity打包会在输出目录下寻找.manifest文件,以实现增量打包.  故打包位置最好固定,打完后复制一份到其他目录
        /// 想要以某个版本为基础,增量打包,需要把该版本的资源复制过来(unity特性)
        /// </summary>
        /// <returns></returns>
        public string BuildOutputCachePath => $"{BuildOutputRoot}/{BuildTarget.ToString()}/{BuildSetting.OutputCache}";

        public string CopyOutputPath => $"{BuildOutputRoot}/{BuildTarget.ToString()}/{AppVersion}/{ResVersion}";

        public BuildAssetBundleOptions GetBuildOptions()
        {
            BuildAssetBundleOptions opt = BuildAssetBundleOptions.None;
            opt |= BuildAssetBundleOptions
                .StrictMode; //Do not allow the build to succeed if any errors are reporting during it.

            // {
            //     opt |= BuildAssetBundleOptions.DryRunBuild;//演练构建,BuildPipeline.BuildAssetBundles仍然返回有效的Manifest,但是不会生成AssetBundle 
            //     return opt;
            // }

            if (CompressOption == CompressOption.Uncompressed)
                opt |= BuildAssetBundleOptions.UncompressedAssetBundle;
            else if (CompressOption == CompressOption.LZ4)
                opt |= BuildAssetBundleOptions.ChunkBasedCompression;

            if (BuildMode == BuildMode.ForceRebuild)
                opt |= BuildAssetBundleOptions.ForceRebuildAssetBundle; //Force rebuild the asset bundles
            if (DisableWriteTypeTree)
                opt |= BuildAssetBundleOptions
                    .DisableWriteTypeTree; //Do not include type information within the asset bundle (don't write type tree).
            if (IgnoreTypeTreeChanges)
                opt |= BuildAssetBundleOptions
                    .IgnoreTypeTreeChanges; //Ignore the type tree changes when doing the incremental build check.

            opt |= BuildAssetBundleOptions
                .DisableLoadAssetByFileName; //AssetBundle.LoadAsset接收三种参数(路径,名子,带后缀的名子). 禁用有两种增加效率
            opt |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

            return opt;
        }

        public void CheckBuildParameters()
        {
            // 检测当前是否正在构建资源包
            if (UnityEditor.BuildPipeline.isBuildingPlayer)
            {
                throw new Exception("The pipeline is buiding, please try again after finish !");
            }

            // 检测是否有未保存场景
            if (BuildMode != BuildMode.SimulateBuild)
            {
                if (EditorUtil.HasDirtyScenes())
                {
                    throw new Exception("Found unsaved scene !");
                }
            }

            // 检测构建参数合法性
            if (BuildTarget == BuildTarget.NoTarget)
            {
                throw new Exception("Please select the build target platform !");
            }

            if (AppVersion <= 0)
            {
                throw new Exception("AppVersion must be greater than 0 !");
            }

            if (ResVersion <= 0)
            {
                throw new Exception("ResVersion must be greater than 0 !");
            }

            if (BuildMode == BuildMode.ForceRebuild)
            {
                if (AFramework.ResModule.Utilities.FileUtil.DeleteDirectory(BuildOutputCachePath))
                {
                    Debug.Log("Delete build root directory: " + BuildOutputCachePath);
                }
            }

            if (!Directory.Exists(BuildOutputCachePath))
            {
                if (AFramework.ResModule.Utilities.FileUtil.CreateDirectory(BuildOutputCachePath))
                {
                    Debug.Log("Create build root directory: " + BuildOutputCachePath);
                }
            }

            if (Directory.Exists(CopyOutputPath))
            {
                throw new Exception($"This version already exists, please check the version number !");
            }
        }
    }
}