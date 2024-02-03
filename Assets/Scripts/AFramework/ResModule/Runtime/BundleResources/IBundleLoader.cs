using System;
using UnityEngine;

namespace AFramework.ResModule.BundleResources
{
    public interface IBundleLoader
    {
        void LoadBundleAsync(BundleInfo bundleInfo, Action<AssetBundle> action);
        AssetBundle LoadBundle(BundleInfo bundleInfo);
    }
}