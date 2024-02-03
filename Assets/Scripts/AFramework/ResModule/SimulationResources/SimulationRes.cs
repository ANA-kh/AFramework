#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace AFramework.ResModule.SimulationResources
{
    public class SimulationRes : Res
    {
        private static string AssetRoot = "Assets/LoxodonFramework/BundleExamples";
        public SimulationRes(string path, IResManager resManager) : base(path, resManager) { }

        public override IRes Load()
        {
            _result = AssetDatabase.LoadAssetAtPath(GetFullPath(), typeof(Object));
            if (_result == null)
            {
                Debug.LogException(
                    new System.Exception($"Load asset failure.The asset named \"{GetFullPath()}\" is not found."));
            }

            OnFinish();
            return this;
        }

        private string GetFullPath()
        {
            const string target = "BundleExamples";
            var index = _path.IndexOf(target);
            if (index < 0)
            {
                Debug.LogError("路径匹配index<0");
                index = 0;
            }
            else
            {
                index += target.Length;
            }

            var relativePath = _path.Substring(index);
            return System.IO.Path.Combine(AssetRoot, relativePath.TrimStart('/'));
        }

        protected override IEnumerator CoLoad()
        {
            _result = AssetDatabase.LoadAssetAtPath(GetFullPath(), typeof(Object));
            if (_result == null)
            {
                Debug.LogException(
                    new System.Exception($"Load asset failure.The asset named \"{GetFullPath()}\" is not found."));
            }

            OnFinish();
            yield break;
        }
    }
}
#endif