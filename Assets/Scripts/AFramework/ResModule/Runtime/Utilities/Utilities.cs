using System;
using System.Text;

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
}