using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AFramework.ResModule.BundleResources;
using AFramework.ResModule.Editor.BundleModifier;
using AFramework.ResModule.Editor.Security;
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

    public class BuildAssetSetting : ScriptableObject
    {
        
        public string OutputPath;                   //输出路径
        public BuildTarget BuildTarget;             //构建平台
        public CompressOptions Compression;         //压缩方式
        public bool forceRebuild;                   //强制重建
        public BuildAssetBundleOptions BuildOptions;//构建选项 TODO
        public string DataVersion;                  //数据版本号   
        public bool CopyToStreaming;                //拷贝到StreamingAssets
        public bool Encryption;                     //加密
        public string Algorithm;                    //加密算法
        public string KEY;                          //加密密钥   GenerateKey
        public string IV;                           //加密向量   GenerateIV
        public EncryptionFilterType FilterType;     //加密过滤类型
        public string filterExpression;             //加密过滤 正则表达式
        public string bundleNames;                  //加密过滤 BundleName列表
        public bool useHashFilename;                //使用Hash文件名
        
        public List<IBundleModifier> CreateBundleModifierChain()
        {
            List<IBundleModifier> bundleModifierChain = new List<IBundleModifier>();
            
            
            //TODO 加密
            bundleModifierChain.Add(new CryptographBundleModifier(new AESCryptograph(this.KEY, this.IV), null));

            if (this.useHashFilename)
                bundleModifierChain.Add(new HashFilenameBundleModifier());

            return bundleModifierChain;
        }

        public virtual void ClearFromStreamingAssets()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                DirectoryInfo dir = new DirectoryInfo(BundleUtil.ReadOnlyDirectory);
                if (dir.Exists)
                    dir.Delete(true);
                AssetDatabase.Refresh();
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        public virtual void CopyToStreamingAssets()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                //TODO 打包逻辑
                BundleBuilder builder = new BundleBuilder();

                DirectoryInfo src = new DirectoryInfo(builder.GetVersionOutput(this.OutputPath, this.BuildTarget, this.DataVersion));
                DirectoryInfo dest = new DirectoryInfo(BundleUtil.ReadOnlyDirectory);

                if (dest.Exists)
                    dest.Delete(true);
                if (!dest.Exists)
                    dest.Create();

                BundleManifest manifest = builder.CopyAssetBundleAndManifest(src, dest);
                if (manifest != null)
                    Debug.LogFormat("Copy AssetBundles success.");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogFormat("Copy AssetBundles failure. Error:{0}", e);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
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