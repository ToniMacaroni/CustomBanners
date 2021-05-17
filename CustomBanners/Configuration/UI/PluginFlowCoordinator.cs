using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeatSaberMarkupLanguage;
using CustomBanners.Configuration.UI.Views;
using CustomBanners.Loaders;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using Zenject;
using Plugin = CustomBanners.Plugin;

namespace CustomBanners.Configuration.UI
{
    internal class PluginFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator _mainFlowCoordinator;

        private ImageListView _imageListView;
        private SettingsView _settingsView;
        private PluginConfig _config;
        private BannerManager _bannerManager;
        private Initializer _initializer;

        [Inject]
        private void Construct(MainFlowCoordinator mainFlowCoordinator, ImageListView imageListView,
            SettingsView settingsView, PluginConfig config, BannerManager bannerManager, Initializer initializer)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _imageListView = imageListView;
            _settingsView = settingsView;
            _config = config;
            _bannerManager = bannerManager;
            _initializer = initializer;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle("Custom Banners");
                showBackButton = true;
                ProvideInitialViewControllers(_imageListView, null, _settingsView);
            }

            _imageListView.OnBannerSelected += OnSelectedBannerChanged;

            _settingsView.OnModToggled += ToggleMod;
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            Cleanup();

            base.BackButtonWasPressed(topViewController);
            _mainFlowCoordinator.DismissFlowCoordinator(this);
        }

        private void Cleanup()
        {
            _imageListView.OnBannerSelected -= OnSelectedBannerChanged;

            _settingsView.OnModToggled -= ToggleMod;
        }

        private void OnSelectedBannerChanged(int idx)
        {
            _settingsView.SelectBanner(idx);
        }

        private void ToggleMod(bool modEnabled)
        {
            if (modEnabled)
            {
                _initializer.Initialize();
                _bannerManager.Enable();
            }
            else
            {
                _bannerManager.Disable();
            }
        }
    }
}