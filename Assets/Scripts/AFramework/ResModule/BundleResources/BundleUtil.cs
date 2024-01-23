using System.IO;
using UnityEngine;

namespace AFramework.ResModule.BundleResources
{
    public class BundleUtil
    {
        public readonly static string streamingAssetsPath = Application.streamingAssetsPath;
        public readonly static string persistentDataPath = Application.persistentDataPath;
        public readonly static string RootPath = BundleSetting.BundleRoot;
        public static string StorableDirectory { get; }
        public static string ReadOnlyDirectory { get; }

        //TODO  此处,在Editor模式只有编译后才会重新调用构造.  在Play时,每次都会调用构造   且play模式静态构造里改变静态成员,会影响Editor
        static BundleUtil()
        {
            StorableDirectory = persistentDataPath + "/" + RootPath + "/";
            ReadOnlyDirectory = streamingAssetsPath + "/" + RootPath + "/";
        }
        
        public static string GetBasePath(BundleInfo bundleInfo)
        {
            if (ExistsInStorableDirectory(bundleInfo.Filename))
                return StorableDirectory;

            if (ExistsInReadOnlyDirectory(bundleInfo.Filename))
                return ReadOnlyDirectory;

            return null;
        }

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
    }
}