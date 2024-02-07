using System.IO;
using System.Text;
using UnityEngine;

namespace AFramework.ResModule.Utilities
{
    public static class FileUtil
    {
        /// <summary>
        /// 项目根目录:Assets的上一级目录
        /// </summary>
        /// <returns></returns>
        public static string GetProjectPath()
        {
            var projectPath = System.IO.Path.GetDirectoryName(Application.dataPath);
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

        public static void WriteAllText(string filePath, string content)
        {
            CreateDirectory(filePath);

            byte[] bytes = Encoding.UTF8.GetBytes(content);
            File.WriteAllBytes(filePath, bytes); //避免写入BOM标记
        }

        public static void CopyDirectory(string sourceDir, string destDir, bool recursive = false)
        {
            CreateDirectory(destDir);

            // copy files
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = PathUtility.CombinePaths(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            // Recursively copy all subDirectories.
            if (recursive)
            {
                foreach (string dir in Directory.GetDirectories(sourceDir))
                {
                    string destDir2 = PathUtility.CombinePaths(destDir, Path.GetFileName(dir));
                    CopyDirectory(dir, destDir2, true);
                }
            }
        }
    }
}