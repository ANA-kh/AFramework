using System;
using System.Collections.Generic;
using System.IO;
using AFramework.ResModule.Editor.BundleModifier;
using UnityEditor;
using UnityEngine;

namespace AFramework.ResModule.Editor
{
    public class BuildAssetBundle
    {
        public static BuildAssetSetting BuildAssetSetting;

        static BuildAssetBundle()
        {
            BuildAssetSetting = Resources.Load<BuildAssetSetting>("BuildAssetSetting");
            if (BuildAssetSetting == null)
            {
                Debug.LogError("BuildAssetSetting is null");
                return;
            }
        }
        
        [MenuItem("Build/BuildAssetBundle")]
        public static void BuildAssetBundles()
        {
            string path = BuildAssetSetting.OutputPath;
            
            if (string.IsNullOrEmpty(path))
            {
                //BrowseOutputFolder();
                Debug.LogError("OutputPath is null");
                return;
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            
            BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle;
            switch (BuildAssetSetting.Compression)
            {
                case CompressOptions.Uncompressed:
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                    break;
                case CompressOptions.ChunkBasedCompression:
                    options |= BuildAssetBundleOptions.ChunkBasedCompression;
                    break;
            }

            if (BuildAssetSetting.forceRebuild)
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;

            options |= BuildAssetSetting.BuildOptions;
            
            List<IBundleModifier> bundleModifierChain = BuildAssetSetting.CreateBundleModifierChain();
            BundleBuilder builder = new BundleBuilder();
            builder.Build(path, BuildAssetSetting.BuildTarget, options, BuildAssetSetting.DataVersion, bundleModifierChain);

#if UNITY_5_6_OR_NEWER
            if ((options & BuildAssetBundleOptions.DryRunBuild) > 0)
            {
                Debug.LogFormat("Dry Build OK.");
                return;
            }
#endif

            DirectoryInfo dir = new DirectoryInfo(builder.GetPlatformOutput(path, BuildAssetSetting.BuildTarget));
            try
            {
                //open the folder 
                EditorUtility.OpenWithDefaultApp(dir.FullName);
            }
            catch (Exception) { }

            Debug.LogFormat("Build OK.Please check the folder:{0}", dir.FullName);

            if (!BuildAssetSetting.CopyToStreaming)
                return;

            BuildAssetSetting.CopyToStreamingAssets();
        }
        
    }

    public enum CompressOptions
    {
        Uncompressed = 0,
        
        StandardCompression,/*LZMA*/
        
        ChunkBasedCompression/*LZ4*/
    }
    
    public enum EncryptionFilterType
    {
        All,//全部加密
        RegularExpression,//正则表达式
        BundleNameList//指定文件加密
    }
}