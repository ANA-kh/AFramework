using AFramework.ResModule.Utilities;
using UnityEngine;
using Path = System.IO.Path;

namespace AFramework.Editor.Builder
{
    public static class FileUtil
    {
        public static string GetProjectPath()
        {
            var projectPath = Path.GetDirectoryName(Application.dataPath);
            return PathUtility.GetRegularPath(projectPath);
        }
    }
}