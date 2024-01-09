// using System;
// using System.Collections;
// using AFramework.ResModule.Utilities;
// using UnityEngine;
// using Object = UnityEngine.Object;
//
// namespace AFramework.ResModule.LocalResources
// {
//     public interface IResourceManager
//     {
//         T LoadAsset<T>(string name) where T : Object;
//         IProgressResult<float,T> LoadAssetAsync<T>(string name) where T : Object;
//     }
//
//     public class LocalResourceManager : IResourceManager
//     {
//         public Object LoadAsset(string name)
//         {
//             if (string.IsNullOrEmpty(name))
//                 throw new System.ArgumentNullException("name", "The name is null or empty!");
//             
//             return Resources.Load(Path.GetFilePathWithoutExtension(name));
//         }
//         
//         public T LoadAsset<T>(string name) where T : Object
//         {
//             if (string.IsNullOrEmpty(name))
//                 throw new System.ArgumentNullException("name", "The name is null or empty!");
//             
//             return Resources.Load<T>(Path.GetFilePathWithoutExtension(name));
//         }
//         
//         public IProgressResult<float,Object> LoadAssetAsync(string name,System.Type type)
//         {
//             try
//             {
//                 if (string.IsNullOrEmpty(name))
//                     throw new System.ArgumentNullException("name", "The name is null or empty!");
//                 
//                 if (type == null)
//                     throw new System.ArgumentNullException("type", "The type is null!");
//                 var result = new ProgressResult<float, Object>();
//                 CoroutineRunner.MStartCoroutine(DoLoadAssetAsync(result, name, type));
//                 return result;
//             }
//             catch (Exception e)
//             {
//                 return new ImmutableProgressResult<float, Object>(e, 1f);
//             }
//         }
//         
//         public IProgressResult<float,T> LoadAssetAsync<T>(string name) where T : Object
//         {
//             try
//             {
//                 if (string.IsNullOrEmpty(name))
//                     throw new System.ArgumentNullException("name", "The name is null or empty!");
//                 
//                 var result = new ProgressResult<float, T>();
//                 CoroutineRunner.MStartCoroutine(DoLoadAssetAsync(result, name));
//                 return result;
//             }
//             catch (Exception e)
//             {
//                 return new ImmutableProgressResult<float, T>(e, 1f);
//             }
//         }
//
//         private IEnumerator DoLoadAssetAsync(IProgressPromise<float, Object> promise, string name, Type type)
//         {
//             var fullName = Path.GetFilePathWithoutExtension(name);
//             var request = Resources.LoadAsync(fullName, type);
//             while (!request.isDone)
//             {
//                 promise.UpdateProgress(request.progress);
//                 yield return null;
//             }
//             
//             promise.UpdateProgress(1f);
//             var asset = request.asset;
//             if (asset == null)
//             {
//                 var exception = new Exception($"Load asset failure.The asset named \"{name}\" is not found.");
//                 Debug.LogException(exception);
//                 promise.SetException(exception);
//             }
//             else
//                 promise.SetResult(asset);
//         }
//         
//         private IEnumerator DoLoadAssetAsync<T>(IProgressPromise<float, T> promise, string name) where T : Object
//         {
//             var fullName = Path.GetFilePathWithoutExtension(name);
//             var request = Resources.LoadAsync<T>(fullName);
//             while (!request.isDone)
//             {
//                 promise.UpdateProgress(request.progress);
//                 yield return null;
//             }
//             
//             promise.UpdateProgress(1f);
//             var asset = (T)request.asset;
//             if (asset == null)
//             {
//                 var exception = new Exception($"Load asset failure.The asset named \"{name}\" is not found.");
//                 Debug.LogException(exception);
//                 promise.SetException(exception);
//             }
//             else
//                 promise.SetResult(asset);
//         }
//     }
// }