using System.IO;
using AFramework.ResModule.Utilities;
using UnityEngine;
using Path = System.IO.Path;

namespace AFramework.ResModule.Editor.Builder
{
    public static class FileUtil
    {
        /// <summary>
        /// 项目根目录:Assets的上一级目录
        /// </summary>
        /// <returns></returns>
        public static string GetProjectPath()
        {
            var projectPath = Path.GetDirectoryName(Application.dataPath);
            return PathUtility.GetRegularPath(projectPath);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        public static bool CreateDirectory(string directory)
        {
            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除文件夹及子目录
        /// </summary>
        public static bool DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取文件大小（字节数）
        /// </summary>
        public static long GetFileSize(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }
    }
}