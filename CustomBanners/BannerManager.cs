using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using CustomBanners.Configuration;
using CustomBanners.Loaders;
using Newtonsoft.Json;
using SiraUtil.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CustomBanners
{
    internal class BannerManager : IDisposable
    {
        public bool BannersEnabled
        {
            get => _bannersEnabled;
            set
            {
                _bannersEnabled = value;
                if(_bannersEnabled) Enable();
                else Disable();
            }
        }

        private readonly List<Banner> _banners = new List<Banner>();

        private readonly SiraLog _logger;
        private readonly PluginConfig _config;
        private readonly SwitchManager _switchManager;

        private GameObject _container;
        private Vector3 _ogPosition;
        private Transform _parent;

        private bool _bannersEnabled;
        private Scene _menuScene;

        private BannerManager(SiraLog logger, PluginConfig config, SwitchManager switchManager)
        {
            _logger = logger;
            _config = config;
            _switchManager = switchManager;
        }

        public void InitBanners(GameObject container)
        {
            _container = Object.Instantiate(container);
            _container.name = "CustomBanners Container";

            _menuScene = SceneManager.GetActiveScene();

            var bannerRenderers = _container.GetComponentsInChildren<Renderer>().Where(x=>x.name=="Flag").ToArray();

            _parent = bannerRenderers[0].transform.parent.parent;
            _ogPosition = _parent.position;

            if (bannerRenderers.Length < 2)
            {
                _logger.Error("Banners not found");
                return;
            }

            SetupBanner(bannerRenderers[0], _config.RightBanner);
            SetupBanner(bannerRenderers[1], _config.LeftBanner);

            SetPosition(_config.Position);
            SetSize(_config.Size);

            BannersEnabled = _config.IsEnabled;

            SetScope(_config.EnableInSong);

            _switchManager.SetBanners(_banners);
        }

        public void SetPosition(float pos)
        {
            foreach (var banner in _banners)
            {
                banner.ClothActive = false;
            }

            _parent.position = _ogPosition + new Vector3(0, 0, pos);

            foreach (var banner in _banners)
            {
                banner.ClothActive = true;
            }
        }

        public void SetSize(float sizeMultiplier)
        {
            foreach (var banner in _banners)
            {
                banner.ClothActive = false;
                banner.Transform.parent.localScale = Vector3.one * sizeMultiplier;
                banner.ClothActive = true;
            }
        }

        public void SetWindStrength(float strength)
        {
            foreach (var banner in _banners)
            {
                banner.ClothActive = false;
                banner.WindStrength = strength;
                banner.ClothActive = true;
            }
        }

        public void SetScope(bool showInSong)
        {
            if (!showInSong)
            {
                SceneManager.MoveGameObjectToScene(_container, _menuScene);
                return;
            }

            Object.DontDestroyOnLoad(_container);
        }

        public void HideHighlighters()
        {
            foreach (var banner in _banners)
            {
                banner.HighlighterActive = false;
            }
        }

        public Banner GetBanner(EBannerType bannerType)
        {
            return _banners[(int) bannerType];
        }

        public void Enable()
        {
            _container.SetActive(true);
        }

        public void Disable()
        {
            _container.SetActive(false);
        }

        public void Dispose()
        {
            Object.DestroyImmediate(_container);
        }

        private void SetupBanner(Renderer renderer, BannerConfig bannerConfig)
        {
            var banner = new Banner(renderer, bannerConfig);
            _banners.Add(banner);
        }
    }
}