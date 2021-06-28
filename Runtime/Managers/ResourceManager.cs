using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WingjoyFramework.UIFramework.Runtime.Managers
{
    public static class ResourceManager
    {
        public delegate void LoadAssetSuccessCallBack(Object obj);
        private static Dictionary<string,AsyncOperationHandle> _Handles;
        public async static Task<T> LoadAssetAsync<T>(string path) where T : Object
        {
            if (_Handles ==null)
            {
                _Handles = new Dictionary<string, AsyncOperationHandle>();
            }
            AsyncOperationHandle handle;
            if(_Handles.TryGetValue(path, out handle))
            {
                return handle.Result as T;
            }
            else
            {
                handle = Addressables.LoadAssetAsync<T>(path);
                await handle.Task;
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    _Handles.Add(path, handle);
                    return handle.Result as T;
                }
                return null;
            }
      
        }



        /// <summary>
        /// 返回一个资源实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="loadAssetSuccessCallBack"></param>
        public static async void LoadAssetAsync<T>(string path, LoadAssetSuccessCallBack loadAssetSuccessCallBack) where T : Object
        {
            var obj= await LoadAssetAsync<T>(path);
            if (obj != null)
            {
                loadAssetSuccessCallBack?.Invoke(obj);
            }
            else
            {
                throw new System.ArgumentException($"路径{path}不存在");
            }
        }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="path"></param>
        public static void ReleaseAsset(string path)
        {
            AsyncOperationHandle handle;
            if( _Handles.TryGetValue(path, out handle))
            {
                Addressables.Release(handle);
            }
        }
    }
}
