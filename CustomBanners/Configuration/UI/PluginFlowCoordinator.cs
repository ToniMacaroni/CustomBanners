﻿using CustomBanners.Configuration.UI.Views;
using CustomBanners.Loaders;
using HMUI;
using Zenject;

namespace CustomBanners.Configuration.UI
{
    internal class PluginFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator _mainFlowCoordinator;

        private ImageListView _imageListView;
        private SettingsView _settingsView;
        private GlobalSettingsView _globalSettingsView;
        private PluginConfig _config;
        private BannerManager _bannerManager;
        private Initializer _initializer;

        [Inject]
        private void Construct(MainFlowCoordinator mainFlowCoordinator, ImageListView imageListView,
            SettingsView settingsView, GlobalSettingsView globalSettingsView, PluginConfig config, BannerManager bannerManager, Initializer initializer)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _imageListView = imageListView;
            _settingsView = settingsView;
            _globalSettingsView = globalSettingsView;
            _config = config;
            _bannerManager = bannerManager;
            _initializer = initializer;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle("Custom Banners <color=#000000EE><size=4><voffset=0.1em> by Toni Macaroni</voffset></size></color>");
                showBackButton = true;
                ProvideInitialViewControllers(_imageListView, _globalSettingsView, _settingsView);
            }

            _imageListView.OnBannerSelected += OnSelectedBannerChanged;
            _imageListView.OnImageChanged += OnSelectedTextureChanged;

            _globalSettingsView.OnModToggled += ToggleMod;
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            Cleanup();

            base.BackButtonWasPressed(topViewController);
            _mainFlowCoordinator.DismissFlowCoordinator(this);
            _bannerManager.HideHighlighters();
        }

        private void Cleanup()
        {
            _imageListView.OnBannerSelected -= OnSelectedBannerChanged;
            _imageListView.OnImageChanged -= OnSelectedTextureChanged;

            _globalSettingsView.OnModToggled -= ToggleMod;
        }

        private void OnSelectedBannerChanged(int idx)
        {
            _settingsView.SelectBanner(idx);
        }

        private void OnSelectedTextureChanged()
        {
            _settingsView.Update();
        }

        private void ToggleMod(bool modEnabled)
        {
            if (modEnabled)
            {
                _initializer.Initialize();
            }

            _bannerManager.BannersEnabled = modEnabled;
        }
    }
}