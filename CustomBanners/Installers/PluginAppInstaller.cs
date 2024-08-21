using CustomBanners.Animations;
using CustomBanners.Configuration;
using CustomBanners.Loaders;
using Zenject;

namespace CustomBanners.Installers
{
    internal class PluginAppInstaller : Installer
    {
        private readonly PluginConfig _config;

        private PluginAppInstaller(PluginConfig config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle();

            Container.Bind<InternalAssetLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<ImageLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<BannerAnimationStateUpdater>().AsSingle();
        }
    }
}