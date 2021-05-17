using System;
using System.Collections;
using System.Threading.Tasks;
using CustomBanners.Helpers;
using SiraUtil.Tools;
using UnityEngine;

namespace CustomBanners.Loaders
{
    internal class InternalAssetLoader
    {
        private static readonly string BundlePath = "Resources.assets";

        private readonly SiraLog _logger;

        public Material Material { get; private set; }

        public bool IsLoaded { get; private set; }

        private InternalAssetLoader(SiraLog logger)
        {
            _logger = logger;
        }

        public async Task LoadAsync()
        {
            if (IsLoaded) return;

            var loader = new EmbeddedAssetBundleLoader<Material>(BundlePath, "_BannerMat");
            var loadResult = await loader.LoadAsync();
            if (!loadResult.Success) return;
            Material = loadResult.Asset;

            IsLoaded = true;
        }
    }
}