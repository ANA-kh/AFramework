using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFramework.Editor.Builder
{
    public class BuildSO : ScriptableObject
    {
        public List<BuildFilter> BuildFilters = new List<BuildFilter>();
        private static string DefaultPath = "Assets/Scripts/AFramework/Editor/Builder/BuildSO.asset";

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