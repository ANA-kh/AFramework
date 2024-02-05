namespace AFramework.ResModule.Editor.Builder
{
    public class BuildSetting
    {
        public static string OutputCache = "OutputCache"; //BundleBuild输出目录,缓存目录

        public static string DefaultBuildOutputRoot()
        {
            return $"{FileUtil.GetProjectPath()}/ABundles";
        }
    }
}