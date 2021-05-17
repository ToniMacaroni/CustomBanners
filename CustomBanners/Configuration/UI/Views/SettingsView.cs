using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomBanners.Loaders;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;


namespace CustomBanners.Configuration.UI.Views
{
    [ViewDefinition("CustomPillows.Configuration.UI.Views.SettingsView")]
    [HotReload(RelativePathToLayout = @"SettingsView")]
    internal class SettingsView : BSMLAutomaticViewController
    {
        public event Action<bool> OnModToggled;

        private SiraLog _logger;
        private PluginConfig _config;
        private BannerManager _bannerManager;

        private BannerManager.Banner _selectedBanner;

        [UIParams] private readonly BSMLParserParams _parserParams = null;

        [Inject]
        private void Construct(SiraLog logger, PluginConfig config, BannerManager bannerManager)
        {
            _logger = logger;
            _config = config;
            _bannerManager = bannerManager;
        }

        public void SelectBanner(int idx)
        {
            _selectedBanner = _bannerManager.GetBanner((EBannerType) idx);
            _parserParams.EmitEvent("update");
            Debug.LogError("Changed Selection " + _selectedBanner.GlowEnabled);
        }

        private bool IsGlowEnabled
        {
            get => _selectedBanner?.GlowEnabled ?? false;
            set
            {
                if (_selectedBanner == null) return;
                _selectedBanner.GlowEnabled = value;
            }
        }

        private bool ShouldTint
        {
            get => _selectedBanner?.ShouldTint ?? false;
            set
            {
                if (_selectedBanner == null) return;
                _selectedBanner.ShouldTint = value;
            }
        }

        private bool RandomFluctuationEnabled
        {
            get => _selectedBanner?.RandomFluctuationEnabled ?? false;
            set
            {
                if (_selectedBanner == null) return;
                _selectedBanner.RandomFluctuationEnabled = value;
            }
        }

        private bool IsModEnabled
        {
            get => _config.IsEnabled;
            set
            {
                _config.IsEnabled = value;
                OnModToggled?.Invoke(value);
            }
        }
    }
}
