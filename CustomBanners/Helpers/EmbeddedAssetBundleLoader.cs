using System.Threading.Tasks;
using UnityEngine;

namespace CustomBanners.Helpers
{
    public class EmbeddedAssetBundleLoader<T> where T : Object
    {
        private readonly string _relativePath;
        private readonly string _assetName;

        public EmbeddedAssetBundleLoader(string relativePath, string assetName)
        {
            _relativePath = relativePath;
            _assetName = assetName;
        }

        public async Task<BundleLoadResult> LoadAsync()
        {
            var taskSource = new TaskCompletionSource<BundleLoadResult>();
            var result = new BundleLoadResult();

            var fileData = await EmbeddedAssetUtilities.ReadAsync(_relativePath);
            if (!fileData.Success) return result;

            AssetBundleCreateRequest asetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(fileData.Data);
            asetBundleCreateRequest.completed += delegate
            {
                AssetBundle assetBundle = asetBundleCreateRequest.assetBundle;
                try
                {
                    AssetBundleRequest assetBundleRequest = assetBundle.LoadAssetAsync<T>(_assetName);
                    assetBundleRequest.completed += delegate
                    {
                        T asset = (T) assetBundleRequest.asset;
                        assetBundle.Unload(asset==null);

                        result.Success = true;
                        result.Asset = asset;

                        taskSource.TrySetResult(result);
                    };
                }
                catch
                {
                    taskSource.TrySetResult(result);
                }
            };
            return await taskSource.Task;
        }

        public struct BundleLoadResult
        {
            public bool Success;
            public T Asset;
        }
    }
}