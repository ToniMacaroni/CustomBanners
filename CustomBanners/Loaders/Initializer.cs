using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CustomBanners;
using CustomBanners.Configuration;
using CustomBanners.Helpers;
using IPA.Utilities;
using IPA.Utilities.Async;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;
using File = System.IO.File;

namespace CustomBanners.Loaders
{
    class Initializer : IInitializable
    {
        private readonly SiraLog _logger;
        private readonly PluginConfig _config;
        private readonly ImageLoader _imageLoader;
        private readonly InternalAssetLoader _prefabLoader;
        private readonly BannerManager _bannerManager;

        public bool IsInitialized { get; private set; }

        private Initializer(SiraLog logger, PluginConfig config, ImageLoader imageLoader, InternalAssetLoader prefabLoader, BannerManager bannerManager)
        {
            _logger = logger;
            _config = config;
            _imageLoader = imageLoader;
            _prefabLoader = prefabLoader;
            _bannerManager = bannerManager;
        }

        public async void Initialize()
        {
            if (!_config.IsEnabled) return;
            await Load();
        }

        public async Task Load()
        {
            if (IsInitialized) return;

            _logger.Info("Initializing");

            await _prefabLoader.LoadAsync();

            _bannerManager.InitBanners(_prefabLoader.FlagContainer);

            await LoadTexture(_config.LeftBanner, EBannerType.Left);
            await LoadTexture(_config.RightBanner, EBannerType.Right);

            _logger.Info("Initialized");

            IsInitialized = true;
        }

        private async Task LoadTexture(BannerConfig config, EBannerType bannerType)
        {
            if (string.IsNullOrWhiteSpace(config.SelectedTexture)) return;

            await _imageLoader.LoadAsync(config.SelectedTexture);

            if (_imageLoader.TryGetImage(config.SelectedTexture, out var tex))
            {
                var banner = _bannerManager.GetBanner(bannerType);
                if (banner == null) return;
                banner.Graphic = tex;
            }
        }
    }
}
