using System;
using System.IO;
using System.Text.RegularExpressions;
using AFramework.ResModule.Utilities;
using UnityEngine;

namespace AFramework.ResModule.BundleResources
{
    public class BundleSetting
    {
        //TODO  此处,在Editor模式只有编译后才会重新调用构造.  在Play时,每次都会调用构造   且play模式静态构造里改变静态成员,会影响Editor
        static BundleSetting()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("BundleSetting");
            if (textAsset == null)
                return;

            using (StringReader reader = new StringReader(textAsset.text))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Replace(" ", "");
                    Match m = Regex.Match(line, @"^([a-zA-Z0-9]+)=([a-zA-Z0-9/_.]+)$", RegexOptions.IgnorePatternWhitespace);
                    if (!m.Success)
                        continue;

                    string key = m.Groups[1].Value;
                    string value = m.Groups[2].Value;
                    switch (key)
                    {
                        case "ManifestFilename":
                            ManifestFilename = value;
                            break;
                        case "RootDir":
                            RootDir = value;
                            break;
                    }
                }
            }
        }

        public static string RootDir = "Product"; //访问资源时的根目录.   persistentDataPath/RootDir   streamingAssetsPath/RootDir
        public static string ManifestFilename = "manifest.json";

        public static string StreamingAssetsRoot => PathUtility.CombinePaths(Application.streamingAssetsPath, RootDir);

        public static string PersistentDataRoot => PathUtility.CombinePaths(Application.persistentDataPath, RootDir);
    }
}