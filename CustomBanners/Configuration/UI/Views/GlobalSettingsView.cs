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
        private SwitchManager _switchManager;

        [UIParams] private readonly BSMLParserParams _parserParams = null;

        [Inject]
        private void Construct(SiraLog logger, PluginConfig config, BannerManager bannerManager, SwitchManager switchManager)
        {
            _logger = logger;
            _config = config;
            _bannerManager = bannerManager;
            _switchManager = switchManager;
        }

        private float WindStrength
        {
            get => _config.WindStrength;
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
                if (!value) _switchManager.UpdateState(false);
                else if(_switchManager.Enabled) _switchManager.UpdateState(true);
                OnModToggled?.Invoke(value);
            }
        }

        private bool EnabledInSong
        {
            get => _config.EnableInSong;
            set
            {
                _config.EnableInSong = value;
                _bannerManager.SetScope(value);
            }
        }

        private bool AutoSwitch
        {
            get => _config.AutoSwitch;
            set
            {
                _config.AutoSwitch = value;
                _switchManager.Enabled = value;
            }
        }

        private float AutoSwitchInterval
        {
            get => _config.AutoSwitchInterval;
            set
            {
                _config.AutoSwitchInterval = value;
                _switchManager.Interval = value;
            }
        }
    }
}
