using System;
using System.IO;
using UnityEngine;

namespace AFramework.ResModule.BundleResources
{
    public class BundleUtil
    {
        public static string StorableDirectory => BundleSetting.PersistentDataRoot;
        public static string ReadOnlyDirectory => BundleSetting.StreamingAssetsRoot;

        public static string GetBasePath(BundleInfo bundleInfo)
        {
            if (ExistsInStorableDirectory(bundleInfo.BundleName))
                return StorableDirectory;

            if (ExistsInReadOnlyDirectory(bundleInfo.BundleName))
                return ReadOnlyDirectory;

            return null;
        }

        public static Manifest LoadManifest()
        {
            if (ExistsInStorableDirectory(BundleSetting.ManifestFilename))
            {
                string json = File.ReadAllText(System.IO.Path.Combine(StorableDirectory, BundleSetting.ManifestFilename));
                return JsonUtility.FromJson<Manifest>(json);
            }
            else
            {
                var text = Resources.Load<TextAsset>(BundleSetting.ManifestFilename);
                if (text)
                {
                    //沙河和builtin都没有manifest文件,需要重装app了
                    throw new Exception("Manifest file not found");
                }

                return JsonUtility.FromJson<Manifest>(text.text);
            }
        }

        #region OldExistsInDirectory  旧的检查文件是否存在的方法

        private static bool ExistsInStorableDirectory(string relativePath)
        {
            string dir = StorableDirectory;
            string fullPath = System.IO.Path.Combine(dir, relativePath);
            if (File.Exists(fullPath))
                return true;

            return false;
        }

        private static bool ExistsInReadOnlyDirectory(string relativePath)
        {
            string dir = ReadOnlyDirectory;
            string fullPath = System.IO.Path.Combine(dir, relativePath);

#if UNITY_ANDROID && !UNITY_EDITOR
//检查Android APK中文件是否存在
            var zipFileName = GetCompressedFileName(fullPath);
            var entryName = fullPath.Substring(fullPath.LastIndexOf("!") + 2);            
            if (GetAndroidAPK(zipFileName).ContainsEntry(entryName))
                return true;
#endif
            if (File.Exists(fullPath))
                return true;

            return false;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
//检查Android APK中文件是否存在

        private static Dictionary<string,ZipFile> zips = new Dictionary<string, ZipFile>();

        public static ZipFile GetAndroidAPK(string path)
        {
            ZipFile zip;
            if (zips.TryGetValue(path, out zip))
                return zip;

            zip = new ZipFile(path);
            zips[path] = zip;
            return zip;
        }

        public static string GetCompressedFileName(string url)
        {
            url = Regex.Replace(url, @"^jar:file:///", "");
            return url.Substring(0, url.LastIndexOf("!"));
        }
#endif

        #endregion
    }
}