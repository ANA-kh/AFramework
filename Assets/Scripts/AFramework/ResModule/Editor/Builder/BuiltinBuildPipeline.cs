using System;
using AFramework.Editor.Builder.BuildContext;
using UnityEditor;

namespace AFramework.Editor.Builder
{
    public class BuiltinBuildPipeline
    {
        public class BuildSetting
        {
            public static string OutputPath = "OutputCache"; //输出目录  
        }

        public void Build(BuildContext.BuildContext buildContext)
        {
            Build_Pre(buildContext);
            Build_CreateMap(buildContext);
            Building(buildContext);
            Build_GenerateManifest(buildContext);
            Build_CopyToOutput(buildContext);
            Build_Post(buildContext);
        }

        private void Build_Pre(BuildContext.BuildContext buildContext) { }

        private void Build_CreateMap(BuildContext.BuildContext buildContext)
        {
            var buildMap = new BuildMap();
            buildMap.CollectAll();
            buildContext.SetContextObject(buildMap);
        }

        private void Building(BuildContext.BuildContext buildContext)
        {
            BuildParameter buildParameter = buildContext.GetContextObject<BuildParameter>();
            BuildMap buildMap = buildContext.GetContextObject<BuildMap>();
            var assetbundleBuilds = buildMap.GetAssetBundleBuilds();
            var buildResult = BuildPipeline.BuildAssetBundles(buildParameter.GetOutputPath(), assetbundleBuilds,
                buildParameter.BuildAssetBundleOptions, buildParameter.BuildTarget);
        }
        
        private void Build_GenerateManifest(BuildContext.BuildContext buildContext)
        {
            
        }
        
        private void Build_CopyToOutput(BuildContext.BuildContext buildContext)
        {
            
        }
        
        private void Build_Post(BuildContext.BuildContext buildContext) { }
    }
}