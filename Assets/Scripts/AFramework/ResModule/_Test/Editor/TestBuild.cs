using System.IO;
using UnityEditor;
using UnityEngine;

namespace ResModule.Test
{
    public class TestBuild
    {
        protected static AssetBundleBuild CreateAssetBundleBuild(string path)
        {
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetBundleName = AFramework.ResModule.Utilities.FileUtil.AbsolutePathToAssetPath(Path.GetFileNameWithoutExtension(path)).Replace('/', '_').ToLower() + ".bundle";
            assetBundleBuild.assetNames = new string[] { path };
            return assetBundleBuild;
        }

        [MenuItem("AFramework/Test/Build")]
        public static void Build()
        {
            var cubePath = "Assets/BundleResources/Models/Assets-HD/Cube.prefab";
            var MeteriaPath = "Assets/BundleResources/Models/Assets-HD/Material.mat";
            AssetBundleBuild[] builds = new AssetBundleBuild[]
            {
                CreateAssetBundleBuild(cubePath),
                CreateAssetBundleBuild(MeteriaPath)
            };

            var outputPath = Application.streamingAssetsPath;
            var buildOptions = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DisableWriteTypeTree;

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            BuildPipeline.BuildAssetBundles(outputPath, builds, buildOptions, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("AFramework/Test/PrintBundleAssets")]
        static void PrintContents()
        {
            if (Selection.activeObject == null)
                return;

            AssetBundle bundle = AssetBundle.LoadFromFile(Application.dataPath + AssetDatabase.GetAssetPath(Selection.activeObject).Remove(0, 6));

            if (bundle != null)
            {
                SerializedObject so = new SerializedObject(bundle);
                System.Text.StringBuilder str = new System.Text.StringBuilder();

                str.Append("Preload table:\n");
                foreach (SerializedProperty d in so.FindProperty("m_PreloadTable"))
                {
                    if (d.objectReferenceValue != null)
                        str.Append("\t<color=green>" + d.objectReferenceValue.name + " " + d.objectReferenceValue.GetType().ToString() + "\n");
                }

                str.Append("Container:\n");
                foreach (SerializedProperty d in so.FindProperty("m_Container"))
                    str.Append("\t" + d.displayName + "\n");

                Debug.Log(str.ToString());
                bundle.Unload(false);
            }
        }
    }
}