using CustomBanners.Configuration.UI;
using CustomBanners.Configuration.UI.Views;
using CustomBanners.Loaders;
using SiraUtil;
using Zenject;

namespace CustomBanners.Installers
{
    internal class PluginMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Initializer>().AsSingle();
            Container.BindInterfacesAndSelfTo<BannerManager>().AsSingle();

            // UI stuff
            Container.Bind<ImageListView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<SettingsView>().FromNewComponentAsViewController().AsSingle();

            Container.Bind<PluginFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        }
    }
}