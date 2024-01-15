using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using AFramework.ResModule.Utilities;
using UnityEngine;

namespace AFramework.ResModule
{
    public abstract class ResManager : IResManager
    {
        //TODO 一段时间检查一下,refCount为0的资源,释放掉     目前,资源生成后会添加到此列表,但默认不会让资源RefCount+1(默认+1则要求调用者必须手动释放),所以此列表中的资源可能有refCount为0的
        protected Dictionary<string, Res> _resMap = new Dictionary<string, Res>();

        public void Retain(Res res)
        {
            if (!_resMap.TryGetValue(res.Path(), out var res1))
            {
                _resMap.Add(res.Path(), res);
            }
        }

        public void Release(Res res)
        {
            if (_resMap.TryGetValue(res.Path(), out var res1))
            {
                _resMap.Remove(res.Path());
            }

            //TODO: LRU
            CoroutineRunner.MStartCoroutine(DelayUnLoadRes(res));
        }

        private IEnumerator DelayUnLoadRes(Res res)
        {
            yield return new WaitForSeconds(5);
            if (res.IsDone && res.RefCount <= 0)
                res.Dispose();
        }

        public virtual Res Load(string path)
        {
            var res = GetOrCreateRes(path);
            if (!res.IsDone)
                res.Load();

            return res;
        }

        protected abstract Res GetOrCreateRes(string path);

        public virtual Res LoadAsync(string path)
        {
            var res = GetOrCreateRes(path);
            if (!res.IsDone)
                res.LoadAsync();

            return res;
        }
    }
}