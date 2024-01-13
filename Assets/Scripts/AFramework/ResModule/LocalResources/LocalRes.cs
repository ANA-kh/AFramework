using System;
using System.Collections;
using AFramework.ResModule.Utilities;
using UnityEngine;

namespace AFramework.ResModule.LocalResources
{
    public class LocalRes:Res
    {
        private string PathWithoutExtension => Path.GetFilePathWithoutExtension(_path);

        public LocalRes(string path, IResManager resManager) : base(path, resManager) { }
        public override IRes Load()
        {
            _result = Resources.Load(PathWithoutExtension);
            OnFinish();
            return this;
        }

        protected override IEnumerator CoLoad()
        {
            var request = Resources.LoadAsync(PathWithoutExtension);
            while (!request.isDone)
            {
                _progressCallback?.Invoke(request.progress);
                yield return null;
            }
            _result = request.asset;
            if (_result == null)
            {
                Debug.LogException(new Exception($"Load asset failure.The asset named \"{PathWithoutExtension}\" is not found."));
            }
            OnFinish();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            Resources.UnloadAsset(_result);
            _result = null;
            base.Dispose(disposing);
        }
    }
}