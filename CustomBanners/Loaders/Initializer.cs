using System.Threading.Tasks;
using CustomBanners.Configuration;
using SiraUtil.Logging;
using Zenject;

namespace CustomBanners.Loaders
{
    class Initializer : IInitializable
    {
        private readonly SiraLog _logger;
        private readonly PluginConfig _config;
        private readonly ImageLoader _imageLoader;
        private readonly InternalAssetLoader _prefabLoader;
        private readonly BannerManager _bannerManager;
        private readonly SwitchManager _switchManager;

        public bool IsInitialized { get; private set; }

        private Initializer(SiraLog logger,
            PluginConfig config,
            ImageLoader imageLoader,
            InternalAssetLoader prefabLoader,
            BannerManager bannerManager,
            SwitchManager switchManager)
        {
            _logger = logger;
            _config = config;
            _imageLoader = imageLoader;
            _prefabLoader = prefabLoader;
            _bannerManager = bannerManager;
            _switchManager = switchManager;
        }

        public async void Initialize()
        {
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

            _switchManager.Init();

            _logger.Info("Initialized");

            IsInitialized = true;
        }

        private async Task LoadTexture(BannerConfig config, EBannerType bannerType)
        {
            if (string.IsNullOrWhiteSpace(config.SelectedTexture)) return;

            await _imageLoader.LoadAsync(config.SelectedTexture, animated: config.SelectedTexture.ToLowerInvariant().EndsWith(".gif"));

            if (_imageLoader.TryGetMedia(config.SelectedTexture, out var media))
            {
                var banner = _bannerManager.GetBanner(bannerType);
                if (banner == null) return;
                banner.SetMedia(media, handleColored:false);
            }
        }
    }
}
