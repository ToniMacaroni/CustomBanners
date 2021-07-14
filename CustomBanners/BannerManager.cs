using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CustomBanners.Configuration;
using CustomBanners.Graphics;
using CustomBanners.Loaders;
using Newtonsoft.Json;
using SiraUtil.Tools;
using UnityEngine;
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

        private GameObject _container;
        private Vector3 _ogPosition;
        private Transform _parent;

        private bool _bannersEnabled;

        private BannerManager(SiraLog logger, PluginConfig config)
        {
            _logger = logger;
            _config = config;
        }

        public void InitBanners(GameObject container)
        {
            _container = Object.Instantiate(container);
            _container.name = "CustomBanners Container";

            var bannerRenderers = _container.GetComponentsInChildren<Renderer>();

            Debug.LogError(bannerRenderers.Length);

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

            BannersEnabled = _config.IsEnabled;
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

        public void SetWindStrength(float strength)
        {
            foreach (var banner in _banners)
            {
                banner.ClothActive = false;
                banner.WindStrength = strength;
                banner.ClothActive = true;
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
        }

        private void SetupBanner(Renderer renderer, BannerConfig bannerConfig)
        {
            var banner = new Banner(renderer, bannerConfig);
            _banners.Add(banner);
        }

        internal class Banner : IGraphicListener
        {
            public GameObject GameObject { get; }

            public Renderer Renderer { get; }

            public Transform Transform { get; }

            private IGraphic _graphic;
            public IGraphic Graphic
            {
                get => _graphic;
                set
                {
                    if (_graphic != null)
                        _graphic.RemoveListener(this);
                    _graphic = value;
                    if (_graphic != null)
                    {
                        _graphic.AddListener(this);
                        UpdateTexture(_graphic.Graphic);
                    }
                }
            }

            public Material Material { get; }

            public bool GlowEnabled
            {
                get => _config.IsGlowEnabled;
                set
                {
                    _config.IsGlowEnabled = value;
                    Material?.SetFloat("_BlendSrcFactorA", value ? 1 : 0);
                    Material?.SetFloat("_BlendDstFactorA", value ? 1 : 0);
                }
            }

            public bool ShouldTint
            {
                get => _config.ShouldTint;
                set
                {
                    _config.ShouldTint = value;
                    Material?.SetColor("_Color", value ? _config.Tint : Color.white);
                }
            }

            public Color Tint
            {
                get => _config.Tint;
                set
                {
                    _config.Tint = value;
                    if (!_config.ShouldTint) return;
                    Material?.SetColor("_Color", value);
                }
            }

            public bool FlipHorizontal
            {
                get => _config.FlipHorizontal;
                set
                {
                    _config.FlipHorizontal = value;
                    Material?.SetTextureScale("_MainTex", value ? new Vector2(-1, 1) : Vector2.one);
                }
            }

            public float Intensity
            {
                get => _config.Intensity;
                set
                {
                    _config.Intensity = value;
                    Material?.SetFloat("_Intensity", value);
                }
            }

            public float WindStrength
            {
                get => _config.WindStrength;
                set
                {
                    _config.WindStrength = value;
                    _cloth.externalAcceleration = new Vector3(0, 0, -value);
                }
            }

            public bool ClothActive
            {
                get => _cloth.enabled;
                set => _cloth.enabled = value;
            }

            private readonly BannerConfig _config;
            private readonly Cloth _cloth;

            public Banner(Renderer renderer, BannerConfig config)
            {
                _config = config;
                GameObject = renderer.gameObject;
                Renderer = renderer;
                Transform = GameObject.transform;
                _cloth = GameObject.GetComponent<Cloth>();
                Material = renderer.material;

                WindStrength = WindStrength;
                GlowEnabled = GlowEnabled;
                ShouldTint = ShouldTint;
                Tint = Tint;
                FlipHorizontal = FlipHorizontal;
                Intensity = Intensity;
            }

            public void ResetTint()
            {
                Tint = TintColor;
            }

            public void UpdateTexture(Texture2D newTexture)
            {
                if (Material == null) return;
                Material.mainTexture = newTexture;
                _config.SelectedTexture = _graphic.Name;
            }

            private static readonly Color TintColor = new Color(0, 0.7529412f, 1);
        }
    }   
}