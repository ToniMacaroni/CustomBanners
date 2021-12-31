using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CustomBanners.Configuration;
using CustomBanners.Loaders;
using SiraUtil.Logging;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace CustomBanners
{
    internal class SwitchManager
    {
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                UpdateState(value);
            }
        }

        public float Interval;

        public bool IsRunning { get; private set; }

        public List<BannerMedia> RandomPool;

        private bool _enabled;
        private readonly PluginConfig _config;
        private readonly ImageLoader _imageLoader;
        private readonly SiraLog _siraLog;
        private List<RandomBanner> _banners;
        private CancellationTokenSource _cancellationTokenSource;

        public SwitchManager(PluginConfig config, ImageLoader imageLoader, SiraLog siraLog)
        {
            _config = config;
            _imageLoader = imageLoader;
            _siraLog = siraLog;
            _enabled = config.AutoSwitch;
            Interval = config.AutoSwitchInterval;
        }

        public void Init()
        {
            Enabled = _enabled;
        }

        public void SetBanners(List<Banner> banners)
        {
            _banners = banners.Select(x=>new RandomBanner(x)).ToList();
        }

        public async void UpdateState(bool enabled)
        {
            if (!enabled || !_config.IsEnabled)
            {
                if (_cancellationTokenSource is { })
                {
                    _cancellationTokenSource.Cancel();
                    IsRunning = false;
                }

                return;
            }

            if (IsRunning) return;

            await _imageLoader.LoadAllAsync();
            RandomPool = _imageLoader.GetRandoms();

            if (RandomPool.Count < 1)
            {
                RandomPool = _imageLoader.Images.Values.ToList();
            }

            if (RandomPool.Count < 3)
            {
                _siraLog.Error("AutoSwitch option needs more than two textures to choose from");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _ = Loop(_cancellationTokenSource.Token);
            IsRunning = true;
        }

        private async Task Loop(CancellationToken token)
        {
            var prev = new int[_banners.Count];

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                foreach (var banner in _banners)
                {
                    var idx = Random.Range(0, RandomPool.Count);
                    while (idx == banner.LastIndex)
                    {
                        idx = Random.Range(0, RandomPool.Count);
                    }

                    banner.LastIndex = idx;
                    banner.Banner.SetMediaAnimated(RandomPool[idx], BannerSwitcher.ETransitionSpeed.Slow);
                }
                await Task.Delay((int)Interval*1000, token);
            }
        }

        internal class RandomBanner
        {
            public Banner Banner;
            public int LastIndex;

            public RandomBanner(Banner banner)
            {
                Banner = banner;
                LastIndex = -1;
            }
        }
    }
}