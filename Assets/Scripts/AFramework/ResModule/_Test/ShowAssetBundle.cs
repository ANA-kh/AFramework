using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AFramework.ResModule._Test
{
    public class ShowAssetBundle : MonoBehaviour
    {
        public List<BundleAndName> bundleAndNames;

        void Start()
        {
            Debug.Log("start");
            for (var index = 0; index < bundleAndNames.Count; index++)
            {
                var v = bundleAndNames[index];
                var bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, v.name));

                if (bundle == null)
                {
                    Debug.Log("Failed to load AssetBundle!");
                    continue;
                }

                v.assetBundle = bundle;
            }
        }

        private void OnDestroy()
        {
            for (var index = 0; index < bundleAndNames.Count; index++)
            {
                var v = bundleAndNames[index];
                v.assetBundle.Unload(true);
            }
        }
    }

    [Serializable]
    public class BundleAndName
    {
        public AssetBundle assetBundle;
        public string name;
    }
}