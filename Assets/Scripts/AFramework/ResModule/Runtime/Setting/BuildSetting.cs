using AFramework.ResModule.Utilities;

namespace AFramework.ResModule.Setting
{
    public class BuildSetting
    {
        public static string OutputCache = "OutputCache"; //BundleBuild输出目录,缓存目录

        public static string ManifestFileName = "Manifest"; //Manifest文件名

        public static string DefaultBuildOutputRoot()
        {
            return $"{FileUtil.GetProjectPath()}/ABundles";
        }
    }
}