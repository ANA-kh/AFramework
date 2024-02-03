using System;
using System.Collections.Generic;
using System.IO;
using AFramework.ResModule.BundleResources;
using AFramework.ResModule.Editor.BundleModifier;
using AFramework.ResModule.Editor.Security;
using UnityEditor;
using UnityEngine;

namespace AFramework.ResModule.Editor
{
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
}