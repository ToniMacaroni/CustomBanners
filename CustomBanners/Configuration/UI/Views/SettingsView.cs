using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomBanners.Loaders;
using SiraUtil.Logging;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace CustomBanners.Configuration.UI.Views
{
    [ViewDefinition("CustomBanners.Configuration.UI.Views.SettingsView.bsml")]
    [HotReload(RelativePathToLayout = @"SettingsView")]
    internal class SettingsView : BSMLAutomaticViewController
    {
        private SiraLog _logger;
        private PluginConfig _config;
        private BannerManager _bannerManager;

        private Banner _selectedBanner;

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
            Update();
        }

        public void Update()
        {
            _parserParams.EmitEvent("update");
        }

        [UIAction("sync-clicked")]
        public void OnSync()
        {
            _bannerManager.GetBanner(EBannerType.Left).Tint = Tint;
            _bannerManager.GetBanner(EBannerType.Right).Tint = Tint;
        }

        [UIAction("tint-reset-clicked")]
        public void OnTintReset()
        {
            _selectedBanner?.ResetTint();
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

        private Color Tint
        {
            get => _selectedBanner?.Tint ?? Color.white;
            set
            {
                if (_selectedBanner == null) return;
                _selectedBanner.Tint = value;
            }
        }

        private bool FlipHorizontal
        {
            get => _selectedBanner?.FlipHorizontal ?? false;
            set
            {
                if (_selectedBanner == null) return;
                _selectedBanner.FlipHorizontal = value;
            }
        }

        private float Intensity
        {
            get => _selectedBanner?.Intensity ?? 1;
            set
            {
                if (_selectedBanner == null) return;
                _selectedBanner.Intensity = value;
            }
        }
    }
}
