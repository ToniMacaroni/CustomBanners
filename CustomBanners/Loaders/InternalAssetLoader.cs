using System.Threading.Tasks;
using CustomBanners.Helpers;
using SiraUtil.Logging;
using UnityEngine;

namespace CustomBanners.Loaders
{
    internal class InternalAssetLoader
    {
        private static readonly string BundlePath = "Resources.assets";

        private readonly SiraLog _logger;

        public GameObject FlagContainer { get; private set; }

        public bool IsLoaded { get; private set; }

        private InternalAssetLoader(SiraLog logger)
        {
            _logger = logger;
        }

        public async Task LoadAsync()
        {
            if (IsLoaded) return;

            var loader = new EmbeddedAssetBundleLoader<GameObject>(BundlePath, "FlagContainer");
            var loadResult = await loader.LoadAsync();
            if (!loadResult.Success) return;
            FlagContainer = loadResult.Asset;

            IsLoaded = true;
        }
    }
}