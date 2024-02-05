using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AFramework.ResModule.Editor.Builder
{
    [Serializable]
    public class BuildFilter
    {
        public bool Active = true;
        public string Path;
        public string Filter = "*.prefab";
        public CollectOption CollectOption = CollectOption.TopDirectory;
        public bool Encrypt = false;
        public BuildType BuildType = BuildType.AssetBundle;

        protected static string BundleExtension = ".bundle";
        protected static HashSet<string> IgnoreFileExtensions = new HashSet<string>()
            { "", ".so", ".dll", ".cs", ".js", ".boo", ".meta", ".cginc", ".hlsl" };

        protected static bool IsIgnoreFile(string fileExtension)
        {
            return IgnoreFileExtensions.Contains(fileExtension);
        }

        protected List<FileInfo> GetFileInfo()
        {
            if (string.IsNullOrEmpty(Path) || !string.IsNullOrEmpty(Filter))
                return null;

            DirectoryInfo directoryInfo = new DirectoryInfo(Path);
            if (!directoryInfo.Exists)
                return null;

            List<FileInfo> result = new List<FileInfo>();
            string[] filters = Filter.Split(';');
            foreach (var filter in filters)
            {
                FileInfo[] prefabs = directoryInfo.GetFiles(filter, SearchOption.AllDirectories);
                foreach (FileInfo file in prefabs)
                {
                    if (IsIgnoreFile(file.Extension))
                        continue;
                    result.Add(file);
                }
            }

            return result;
        }

        internal List<BuildAssetInfo> GetBuildAssetInfos()
        {
            var fileInfos = GetFileInfo();
            if (fileInfos == null)
                return null;

            List<BuildAssetInfo> result = new List<BuildAssetInfo>();
            foreach (var fileInfo in fileInfos)
            {
                BuildAssetInfo buildAssetInfo = new BuildAssetInfo(fileInfo.FullName);
                buildAssetInfo.BundleName = GetAssetBundleName(fileInfo);
                buildAssetInfo.DependAssets = GetAllDependencies(fileInfo.FullName, buildAssetInfo.BundleName);
                result.Add(buildAssetInfo);
            }

            return result;
        }

        protected List<BuildAssetInfo> GetAllDependencies(string assetPath, string dependBundleName)
        {
            string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);
            if (dependencies == null || dependencies.Length == 0)
                return null;

            List<BuildAssetInfo> result = new List<BuildAssetInfo>();
            foreach (var dependency in dependencies)
            {
                if (dependency == assetPath)
                    continue;

                BuildAssetInfo buildAssetInfo = new BuildAssetInfo(dependency, dependBundleName);
                result.Add(buildAssetInfo);
            }

            return result;
        }

        /// <summary>
        /// 自定义AssetBundleName
        /// </summary>
        /// <param name="path"></param>
        protected virtual string GetAssetBundleName(FileInfo fileInfo)
        {
            if (fileInfo == null)
                return "";

            string result;
            switch (CollectOption)
            {
                case CollectOption.TopDirectory:
                    result = $"{Path.Replace('/', '_').ToLower()}{BundleExtension}";
                    break;
                case CollectOption.EachDirectory:
                    result = $"{fileInfo.DirectoryName.Replace('/', '_').ToLower()}{BundleExtension}";
                    break;
                case CollectOption.EachFile:
                    result = $"{fileInfo.FullName.Replace('/', '_').ToLower()}{BundleExtension}";
                    break;
                default:
                    result = "";
                    break;
            }

            return result;
        }

        #region Debug

        [MenuItem("Assets/_BuildBundle/GetAllDependenciesByAssetDataBase/Recursive")]
        public static void GetAllDependenciesByAssetDataBase()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids == null || guids.Length == 0)
                return;

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);
            if (dependencies == null || dependencies.Length == 0)
                return;

            Debug.Log(JsonUtility.ToJson(new StringArrayJsonWrapper() { data = dependencies }, true));
        }

        [MenuItem("Assets/_BuildBundle/GetAllDependenciesByAssetDataBase/NotRecursive")]
        public static void GetAllDependenciesByAssetDataBaseNotRecursive()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids == null || guids.Length == 0)
                return;

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            string[] dependencies = AssetDatabase.GetDependencies(assetPath, false);
            if (dependencies == null || dependencies.Length == 0)
                return;

            Debug.Log(JsonUtility.ToJson(new StringArrayJsonWrapper() { data = dependencies }, true));
        }

        protected class StringArrayJsonWrapper
        {
            public string[] data;
        }

        [MenuItem("Assets/_BuildBundle/GetAllDependenciesByEditorDataBase")]
        public static void GetAllDependenciesByEditorDataBase()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids == null || guids.Length == 0)
                return;

            var baseAsset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guids[0]));
            var dependencies = EditorUtility.CollectDependencies(new UnityEngine.Object[] { baseAsset });
            if (dependencies == null || dependencies.Length == 0)
                return;

            string[] paths = new string[dependencies.Length];
            for (int i = 0; i < dependencies.Length; i++)
            {
                paths[i] = AssetDatabase.GetAssetPath(dependencies[i]);
            }

            Debug.Log(JsonUtility.ToJson(new StringArrayJsonWrapper() { data = paths }, true));
        }

        [MenuItem("Assets/_BuildBundle/BuildTest")]
        public static void BuildTest()
        {
            string[] guids = Selection.assetGUIDs;
            AssetBundleBuild[] builds = new AssetBundleBuild[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                builds[i].assetBundleName = assetPath.Replace('/', '_').ToLower() + BundleExtension;
                builds[i].assetNames = new string[] { assetPath };
            }

            var outputPath = FileUtil.GetProjectPath() + "/ABundles";
            BuildPipeline.BuildAssetBundles(outputPath, builds, BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows);
        }

        #endregion
    }

    public enum BuildType
    {
        Raw,
        AssetBundle,
    }

    public enum CollectOption
    {
        TopDirectory,
        EachDirectory,
        EachFile
    }
}