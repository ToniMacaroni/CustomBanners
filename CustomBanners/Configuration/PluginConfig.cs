using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Data;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CustomBanners.Configuration
{
    internal class PluginConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool EnableInSong { get; set; } = true;

        public bool AutoSwitch { get; set; } = false;
        public bool ConsistentAutoSwitch { get; set; } = false;

        public float AutoSwitchInterval { get; set; } = 5;

        public BannerConfig LeftBanner { get; set; } = new BannerConfig();

        public BannerConfig RightBanner { get; set; } = new BannerConfig();

        public float Position { get; set; } = 0f;
        public float Size { get; set; } = 1f;
        public float WindStrength { get; set; } = 0.5f;

    }
}