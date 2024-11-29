using CustomBanners.Configuration.UI;
using CustomBanners.Configuration.UI.Views;
using CustomBanners.Loaders;
using Zenject;

namespace CustomBanners.Installers
{
    internal class PluginMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Initializer>().AsSingle();
            Container.BindInterfacesAndSelfTo<BannerManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<SwitchManager>().AsSingle();

            // UI stuff
            Container.Bind<ImageListView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<SettingsView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<GlobalSettingsView>().FromNewComponentAsViewController().AsSingle();

            Container.Bind<PluginFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
        }
    }
}