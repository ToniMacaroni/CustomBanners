using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using CustomBanners.Configuration.UI;
using Zenject;

namespace CustomBanners
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly MenuButton _menuButton;
        private readonly MainFlowCoordinator _mainFlowCoordinator;
        private readonly PluginFlowCoordinator _pluginFlowCoordinator;

        private MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, PluginFlowCoordinator pluginFlowCoordinator)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _pluginFlowCoordinator = pluginFlowCoordinator;
            _menuButton = new MenuButton("Custom Banners", "BAAAAAAAANNNNNNNNNNNEEEERRRSSS!!1!1!!", ShowPillowFlowCoordinator);
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void Dispose()
        {
            if (MenuButtons.instance != null)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
            }
        }

        private void ShowPillowFlowCoordinator()
        {
            _mainFlowCoordinator.PresentFlowCoordinator(_pluginFlowCoordinator);
        }
    }
}
