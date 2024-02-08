using System;
using AFramework.ResModule.BundleResources;
using UnityEngine;

namespace AFramework.ResModule.Test
{
    public class TestLoadAsset : MonoBehaviour
    {
        private BundleManager _resManager;

        private void Awake()
        {
            _resManager = new BundleManager();
        }

        private void Start()
        {
            var result = _resManager.LoadAsync("Assets/BundleResources/Models/Green/Green.prefab");
            result.OnCallback(obj =>
            {
                var go = Instantiate(obj as GameObject);
                go.transform.position = Vector3.zero;
            });
        }
    }
}