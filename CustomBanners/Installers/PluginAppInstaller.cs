using CustomBanners.Animations;
using CustomBanners.Configuration;
using CustomBanners.Loaders;
using SiraUtil;
using Zenject;
using Logger = IPA.Logging.Logger;

namespace CustomBanners.Installers
{
    internal class PluginAppInstaller : Installer
    {
        private readonly PluginConfig _config;
        private readonly Logger _logger;

        private PluginAppInstaller(PluginConfig config, Logger logger)
        {
            _config = config;
            _logger = logger;
        }

        public override void InstallBindings()
        {
            Container.BindLoggerAsSiraLogger(_logger);
            Container.BindInstance(_config).AsSingle();

            Container.Bind<InternalAssetLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<ImageLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<BannerAnimationStateUpdater>().AsSingle();
        }
    }
}