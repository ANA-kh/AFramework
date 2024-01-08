using System;
using System.Collections;
using System.Collections.Generic;
using AFramework.ResModule.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AFramework.ResModule.LocalResources
{
    public class LocalResManager : IResManager
    {
        private Dictionary<string, Res> _resDic = new Dictionary<string, Res>();
        private IResLoader _loader = new LocalResLoader();

        public Object Load(string path)
        {
            if (_resDic.TryGetValue(path, out var res))
            {
                return res.Result;
            }

            res = new Res(path, this);
            _resDic.Add(path, res);
            res.Load();
            return res.Result;
        }

        public IRes<float, Object> LoadAsync(string path)
        {
            if (_resDic.TryGetValue(path, out var res))
            {
                if (res.Exception != null)
                {
                    //TODO 是否需要重新加载  重新加载几次?
                }

                return res;
            }

            res = new Res(path, this);
            res.LoadAsync();
            _resDic.Add(path, res);
            return res;
        }

        public IResLoader GetLoader()
        {
            return _loader;
        }

        public void UnUseRes(Res res)
        {
            _resDic.Remove(res.Path);
        }

        public void Update() { }

        public float GcIntervalTime
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.OSXEditor)
                    return 1f;

                return Debug.isDebugBuild ? 5f : 10f;
            }
        }
    }

    public class LocalResLoader : IResLoader
    {
        public void Unload(Object result)
        {
            Resources.UnloadAsset(result);
        }

        public Object Load(string path, System.Type type)
        {
            throw new NotImplementedException();
        }

        public IEnumerator LoadAsync(IProgressPromise<float, Object> promise, string path, System.Type type)
        {
            var fullName = Path.GetFilePathWithoutExtension(path);
            var request = Resources.LoadAsync(fullName, type);
            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                yield return null;
            }

            promise.UpdateProgress(1f);
            var asset = request.asset;
            if (asset == null)
            {
                var exception = new Exception($"Load asset failure.The asset named \"{path}\" is not found.");
                Debug.LogException(exception);
                promise.SetException(exception);
            }
            else
                promise.SetResult(asset);
        }
    }
}