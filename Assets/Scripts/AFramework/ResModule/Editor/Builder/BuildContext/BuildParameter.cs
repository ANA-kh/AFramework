using UnityEditor;

namespace AFramework.Editor.Builder.BuildContext
{
    public class BuildParameter
    {
        public BuildParameter()
        {
            BuildOutputRoot = FileUtil.GetProjectPath() + "/ABundles";
        }

        public string BuildOutputRoot = "OutputCache";
        public string PackageVersion = "1.0.0";
        public BuildTarget BuildTarget = BuildTarget.StandaloneWindows64;
        public BuildAssetBundleOptions BuildAssetBundleOptions = BuildAssetBundleOptions.None;

        /// <summary>
        /// 输出目录
        /// unity打包会在输出目录下寻找.manifest文件,以实现增量打包.  故打包位置最好固定,打完后复制一份到其他目录
        /// 想要以某个版本为基础,增量打包,需要把该版本的资源复制过来(unity特性)
        /// </summary>
        /// <returns></returns>
        public string GetOutputPath()
        {
            return $"{BuildOutputRoot}/{BuildTarget.ToString()}/{BuiltinBuildPipeline.BuildSetting.OutputPath}";
        }
    }
}