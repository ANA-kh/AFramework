using System;
using System.Collections.Generic;
using AFramework.ResModule.Utilities;
using UnityEditor;
using UnityEngine;
using Path = System.IO.Path;

namespace AFramework.Editor.Builder
{
    public class BuiltinBuildPipeline
    {
        public class BuildSetting
        {
            public static string OutputPath = "OutputCache"; //输出目录  
        }

        public void Build(BuildContext buildContext)
        {
            Build_Pre(buildContext);
            Building(buildContext);
        }

        private void Build_Pre(BuildContext buildContext) { }

        private void Build_CreateMap(BuildContext buildContext)
        {
            var buildMap = new BuildMap();
            buildMap.CollectAll();
            buildContext.SetContextObject(buildMap);
        }

        private void Building(BuildContext buildContext)
        {
            BuildParameter buildParameter = buildContext.GetContextObject<BuildParameter>();
            BuildMap buildMap = buildContext.GetContextObject<BuildMap>();
            var assetbundleBuilds = buildMap.GetAssetBundleBuilds();
            var buildResult = BuildPipeline.BuildAssetBundles(buildParameter.GetOutputPath(), assetbundleBuilds,
                buildParameter.BuildAssetBundleOptions, buildParameter.BuildTarget);
        }
    }

    public class BuildMap
    {
        private Dictionary<string, BuildBundleInfo> _buildBundleInfos = new Dictionary<string, BuildBundleInfo>();
        private Dictionary<string, BuildAssetInfo> _buildAssetInfos = new Dictionary<string, BuildAssetInfo>();

        public AssetBundleBuild[] GetAssetBundleBuilds()
        {
            return null;
        }

        public void CollectAll()
        {
            var buildSo = BuildSO.GetDefaultBuildSo();
            //1.收集所有资源  TODO 主要资源按规则打包.   依赖资源自动打包(规则:若只被单个AssetBundle依赖,则打包到该AssetBundle中. 否则单独打包)
            foreach (var buildFilter in buildSo.BuildFilters)
            {
                if (!buildFilter.Active)
                    continue;
                var buildAssetInfos = buildFilter.GetBuildAssetInfos();
                if (buildAssetInfos == null)
                    continue;

                foreach (var buildAssetInfo in buildAssetInfos)
                {
                    if (!string.IsNullOrEmpty(buildAssetInfo.BundleName)) //main Asset
                    {
                        if (_buildBundleInfos.TryGetValue(buildAssetInfo.BundleName, out var buildBundleInfo))
                        {
                            buildBundleInfo.AddAssets(buildAssetInfo);
                        }
                        else
                        {
                            var newBuildBundleInfo = new BuildBundleInfo();
                            _buildBundleInfos.Add(buildAssetInfo.BundleName, newBuildBundleInfo);
                            newBuildBundleInfo.AddAssets(buildAssetInfo);
                        }
                    }
                    else //depend Asset
                    {
                        _buildAssetInfos.Add(buildAssetInfo.AssetGUID, buildAssetInfo);
                    }
                }
            }
        }
    }

    public class BuildBundleInfo
    {
        private HashSet<BuildAssetInfo> _buildAssetInfos = new HashSet<BuildAssetInfo>();

        public void AddAssets(BuildAssetInfo buildAssetInfo)
        {
            if (buildAssetInfo == null)
            {
                Debug.LogError("BuildAssetInfo is null");
                return;
            }

            if (!_buildAssetInfos.Add(buildAssetInfo))
            {
                Debug.LogError($"[BuildBundleInfo] BuildAssetInfo is already existed.  :{buildAssetInfo.AssetPath}");
            }
        }
    }

    public class BuildAssetInfo
    {
        public BuildAssetInfo(string assetPath)
        {
            AssetPath = assetPath;
            AssetGUID = AssetDatabase.AssetPathToGUID(assetPath);
            AssetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            DependBundleNames = new HashSet<string>();
        }

        public BuildAssetInfo(string assetPath, string dependBundleName)
        {
            AssetPath = assetPath;
            AssetGUID = AssetDatabase.AssetPathToGUID(assetPath);
            AssetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            DependBundleNames = new HashSet<string> { dependBundleName };
        }

        public string AssetPath;
        public string AssetGUID;
        public System.Type AssetType;
        /// <summary>
        /// 包名,决定了打进那个包
        /// </summary>
        public string BundleName;
        public List<BuildAssetInfo> DependAssets;
        public HashSet<string> DependBundleNames = new HashSet<string>();
    }

    public class BuildContext
    {
        private Dictionary<System.Type, object> _contextObjects = new Dictionary<System.Type, object>();

        public T GetContextObject<T>()
        {
            var type = typeof(T);
            if (_contextObjects.TryGetValue(type, out object contextObject))
            {
                return (T)contextObject;
            }
            else
            {
                throw new System.Exception($"Not found context object : {type}");
            }
        }

        public void SetContextObject(object contextObject)
        {
            if (contextObject == null)
                throw new System.ArgumentNullException("contextObject");

            var type = contextObject.GetType();
            if (_contextObjects.ContainsKey(type))
                throw new System.Exception($"Context object {type} is already existed.");

            _contextObjects.Add(type, contextObject);
        }
    }

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

    public class FileUtil
    {
        public static string GetProjectPath()
        {
            var projectPath = Path.GetDirectoryName(Application.dataPath);
            return PathUtility.GetRegularPath(projectPath);
        }
    }
}