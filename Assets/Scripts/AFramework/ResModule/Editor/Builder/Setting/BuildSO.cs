using System.Collections.Generic;
using AFramework.ResModule.Editor.Builder.BuildContext;
using UnityEditor;
using UnityEngine;

namespace AFramework.ResModule.Editor.Builder
{
    public class BuildSO : ScriptableObject
    {
        public BuildParameter BuildParameter = new BuildParameter();
        public List<BuildFilter> BuildFilters = new List<BuildFilter>();
        private static string DefaultPath = "Assets/Resources/BuildSO.asset";

        public static BuildSO GetDefaultBuildSo()
        {
            var buildSo = AssetDatabase.LoadAssetAtPath<BuildSO>(DefaultPath);
            if (buildSo == null)
            {
                buildSo = ScriptableObject.CreateInstance<BuildSO>();
                AssetDatabase.CreateAsset(buildSo, DefaultPath);
                AssetDatabase.SaveAssets();
            }

            return buildSo;
        }
    }
}