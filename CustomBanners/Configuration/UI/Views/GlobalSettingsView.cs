using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Parser;
using SiraUtil.Tools;
using Zenject;


namespace CustomBanners.Configuration.UI.Views
{
    [ViewDefinition("CustomBanners.Configuration.UI.Views.GlobalSettingsView.bsml")]
    [HotReload(RelativePathToLayout = @"GlobalSettingsView")]
    internal class GlobalSettingsView : BSMLAutomaticViewController
    {
        public event Action<bool> OnModToggled;

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

        private float WindStrength
        {
            get => _selectedBanner?.WindStrength ?? 0.5f;
            set => _bannerManager.SetWindStrength(value);
        }

        private float BannerPosition
        {
            get => _config.Position;
            set
            {
                _config.Position = value;
                _bannerManager.SetPosition(value);
            }
        }

        private float BannerSize
        {
            get => _config.Size;
            set
            {
                _config.Size = value;
                _bannerManager.SetSize(value);
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
