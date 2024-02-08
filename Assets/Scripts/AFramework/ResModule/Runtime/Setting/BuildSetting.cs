using AFramework.ResModule.Utilities;
using UnityEngine;

namespace AFramework.ResModule.Setting
{
    public class BuildSetting
    {
        public static string OutputCache = "OutputCache"; //BundleBuild输出目录,缓存目录

        public static string ManifestFileName = "Manifest"; //Manifest文件名

        /// <summary>
        /// 打包文件的存储目录
        /// </summary>
        /// <returns></returns>
        public static string DefaultBuildOutputRoot()
        {
            return $"{FileUtil.GetProjectPath()}/ABundles"; //  AFramework/ABundles
        }
    }
}