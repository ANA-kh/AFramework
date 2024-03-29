using System;
using AFramework.ResModule.Utilities;
using UnityEngine;

namespace AFramework.ResModule.BundleResources
{
    public class FileBundleLoader : IBundleLoader
    {
        public void LoadBundleAsync(BundleInfo bundleInfo, Action<AssetBundle> action)
        {
            var request = AssetBundle.LoadFromFileAsync(GetAbsolutePath(bundleInfo));
            request.completed += operation => { action?.Invoke(request.assetBundle); };
        }

        public AssetBundle LoadBundle(BundleInfo bundleInfo)
        {
            return AssetBundle.LoadFromFile(GetAbsolutePath(bundleInfo));
        }

        public string GetAbsolutePath(BundleInfo bundleInfo)
        {
            var basePath = BundleUtil.GetBasePath(bundleInfo);
            if (basePath == null)
            {
                throw new Exception("bundle not exist");
            }

            //TODO 提取到PathParser   进行真机路径测试,记录结论
            Uri baseUri = new Uri(basePath);
            Uri uri = new Uri(PathUtility.CombinePaths(basePath, bundleInfo.BundleName));
            string path = System.Uri.UnescapeDataString(uri.AbsolutePath);
            if (uri.Scheme.Equals("jar"))
                path = path.Replace("file://", "jar:file://");
            return path;
        }
    }
}