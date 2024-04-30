using System;
using UnityEngine;

namespace AFramework.ResModule._Test
{
    public class TestLoadBundle : MonoBehaviour
    {
        private void Start()
        {
            var cubePath = "Assets/StreamingAssets/cube.prefab";
            var MeteriaPath = "Assets/StreamingAssets/material.mat";
            var cubeBundle = AssetBundle.LoadFromFile(cubePath);
            var meteriaBundle = AssetBundle.LoadFromFile(MeteriaPath);
            GameObject.Instantiate(cubeBundle.LoadAsset<GameObject>("Cube"));
        }
    }
}