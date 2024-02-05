using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace AFramework.ResModule.Utilities
{
    public static class PathUtility
    {
        /// <summary>
        /// 处理不同平台的路径分隔符差异
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRegularPath(string path)
        {
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// x/x/x.xxx -> x/x/x
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveExtension(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            int index = path.LastIndexOf('.');
            if (index > 0)
                return path.Substring(0, index);
            return path;
        }

        public static string CombinePaths(string path1, string path2)
        {
            return StringUtility.Format("{0}/{1}", path1, path2);
        }

        public static string CombinePaths(string path1, string path2, string path3)
        {
            return StringUtility.Format("{0}/{1}/{2}", path1, path2, path3);
        }
    }

    public static class StringUtility
    {
        [ThreadStatic] //此静态变量在线程中不共享,每个线程都有一份
        private static StringBuilder _stringBuilder = new StringBuilder(2048); //减少内存分配与回收

        public static string Format(string format, object arg0)
        {
            _stringBuilder.Length = 0;
            _stringBuilder.AppendFormat(format, arg0);
            return _stringBuilder.ToString();
        }

        public static string Format(string format, object arg0, object arg1)
        {
            _stringBuilder.Length = 0;
            _stringBuilder.AppendFormat(format, arg0, arg1);
            return _stringBuilder.ToString();
        }

        public static string Format(string format, object arg0, object arg1, object arg2)
        {
            _stringBuilder.Length = 0;
            _stringBuilder.AppendFormat(format, arg0, arg1, arg2);
            return _stringBuilder.ToString();
        }

        public static string Format(string format, params object[] args)
        {
            _stringBuilder.Length = 0;
            _stringBuilder.AppendFormat(format, args);
            return _stringBuilder.ToString();
        }
    }

    /// <summary>
    /// 哈希工具类
    /// </summary>
    public static class HashUtility
    {
        private static string ToString(byte[] hashBytes)
        {
            string result = BitConverter.ToString(hashBytes);
            result = result.Replace("-", "");
            return result.ToLower();
        }

        #region SHA1

        /// <summary>
        /// 获取字符串的Hash值
        /// </summary>
        public static string StringSHA1(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            return BytesSHA1(buffer);
        }

        /// <summary>
        /// 获取文件的Hash值
        /// </summary>
        public static string FileSHA1(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return StreamSHA1(fs);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取数据流的Hash值
        /// </summary>
        public static string StreamSHA1(Stream stream)
        {
            // 说明：创建的是SHA1类的实例，生成的是160位的散列码
            HashAlgorithm hash = HashAlgorithm.Create();
            byte[] hashBytes = hash.ComputeHash(stream);
            return ToString(hashBytes);
        }

        /// <summary>
        /// 获取字节数组的Hash值
        /// </summary>
        public static string BytesSHA1(byte[] buffer)
        {
            // 说明：创建的是SHA1类的实例，生成的是160位的散列码
            HashAlgorithm hash = HashAlgorithm.Create();
            byte[] hashBytes = hash.ComputeHash(buffer);
            return ToString(hashBytes);
        }

        #endregion

        #region MD5

        /// <summary>
        /// 获取字符串的MD5
        /// </summary>
        public static string StringMD5(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            return BytesMD5(buffer);
        }

        /// <summary>
        /// 获取文件的MD5
        /// </summary>
        public static string FileMD5(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return StreamMD5(fs);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取数据流的MD5
        /// </summary>
        public static string StreamMD5(Stream stream)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] hashBytes = provider.ComputeHash(stream);
            return ToString(hashBytes);
        }

        /// <summary>
        /// 获取字节数组的MD5
        /// </summary>
        public static string BytesMD5(byte[] buffer)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] hashBytes = provider.ComputeHash(buffer);
            return ToString(hashBytes);
        }

        #endregion

        #region CRC32

        /// <summary>
        /// 获取字符串的CRC32
        /// </summary>
        public static string StringCRC32(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            return BytesCRC32(buffer);
        }

        /// <summary>
        /// 获取文件的CRC32
        /// </summary>
        public static string FileCRC32(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return StreamCRC32(fs);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取数据流的CRC32
        /// </summary>
        public static string StreamCRC32(Stream stream)
        {
            CRC32Algorithm hash = new CRC32Algorithm();
            byte[] hashBytes = hash.ComputeHash(stream);
            return ToString(hashBytes);
        }

        /// <summary>
        /// 获取字节数组的CRC32
        /// </summary>
        public static string BytesCRC32(byte[] buffer)
        {
            CRC32Algorithm hash = new CRC32Algorithm();
            byte[] hashBytes = hash.ComputeHash(buffer);
            return ToString(hashBytes);
        }

        #endregion
    }
}