using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CustomBanners.Configuration;
using CustomBanners.Loaders;
using Newtonsoft.Json;
using SiraUtil.Tools;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CustomBanners
{
    internal class BannerManager : IDisposable
    {
        private readonly List<Banner> _banners = new List<Banner>();

        private readonly SiraLog _logger;
        private readonly PluginConfig _config;

        private readonly Vector3 _ogPosition;
        private readonly Transform _parent;

        private BannerManager(SiraLog logger, PluginConfig config)
        {
            _logger = logger;
            _config = config;

            var bannerRenderers =
                Object.FindObjectsOfType<SkinnedMeshRenderer>().Where(x => x.name == "Flag").ToArray();

            _parent = bannerRenderers[0].transform.parent.parent;
            _ogPosition = _parent.position;

            SetPosition(config.Position);

            if (bannerRenderers.Length < 2)
            {
                logger.Error("Banners not found");
                return;
            }
            
            SetupBanner(bannerRenderers[0], config.RightBanner);
            SetupBanner(bannerRenderers[1], config.LeftBanner);
        }

        public void SetMaterial(Material mat)
        {
            foreach (var banner in _banners)
            {
                banner.SetMaterial(mat);
                banner.IsCustomMaterialEnabled = _config.IsEnabled;
            }
        }

        public void SetPosition(float pos)
        {
            _parent.position = _ogPosition + new Vector3(0, 0, pos);
        }

        public Banner GetBanner(EBannerType bannerType)
        {
            return _banners[(int) bannerType];
        }

        public void Enable()
        {
            foreach (var banner in _banners)
            {
                banner.IsCustomMaterialEnabled = true;
            }
        }

        public void Disable()
        {
            foreach (var banner in _banners)
            {
                banner.IsCustomMaterialEnabled = false;
            }
        }

        public void Dispose()
        {
        }

        private void SetupBanner(SkinnedMeshRenderer renderer, BannerConfig bannerConfig)
        {
            var banner = new Banner(renderer, bannerConfig);
            _banners.Add(banner);
        }

        internal class Banner
        {
            public GameObject GameObject { get; }

            public SkinnedMeshRenderer Renderer { get; }

            public Transform Transform { get; }

            public Texture Texture
            {
                get => Material?.mainTexture;
                set
                {
                    if (Material == null) return;
                    Material.mainTexture = value;
                    _config.SelectedTexture = value.name;
                }
            }

            public Material Material { get; private set; }

            public bool IsCustomMaterialEnabled
            {
                get => _isCustomMaterialEnabled;
                set
                {
                    _isCustomMaterialEnabled = value;
                    if(value) ApplyMaterial();
                    else RevertMaterial();
                }
            }

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
                    Material?.SetColor("_Color", value ? TintColor : Color.white);
                }
            }

            public bool FlipHorizontal
            {
                get => _config.FlipHorizontal;
                set
                {
                    _config.FlipHorizontal = value;
                    Material?.SetTextureScale("_MainTex", value?new Vector2(-1, 1) : Vector2.one);
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

            public bool RandomFluctuationEnabled
            {
                get => _config.RandomFluctuationEnabled;
                set
                {
                    _config.RandomFluctuationEnabled = value;
                    _clothRandomFluctuation.enabled = value;
                }
            }

            private readonly Material _ogMaterial;
            private readonly BannerConfig _config;
            private readonly ClothRandomFluctuation _clothRandomFluctuation;
            private bool _isCustomMaterialEnabled;

            public Banner(SkinnedMeshRenderer renderer, BannerConfig config)
            {
                _config = config;
                GameObject = renderer.gameObject;
                Renderer = renderer;
                Transform = GameObject.transform;
                _ogMaterial = Renderer.material;
                _clothRandomFluctuation = GameObject.GetComponent<ClothRandomFluctuation>();

                RandomFluctuationEnabled = RandomFluctuationEnabled;
            }

            public void RevertMaterial()
            {
                if (Renderer is null || _ogMaterial is null) return;
                Renderer.material = _ogMaterial;
            }

            public void ApplyMaterial()
            {
                if (Renderer is null || Material is null) return;
                Renderer.material = Material;
            }

            public void SetMaterial(Material materialPrefab)
            {
                Material = new Material(materialPrefab);
                GlowEnabled = GlowEnabled;
                ShouldTint = ShouldTint;
                FlipHorizontal = FlipHorizontal;
                Intensity = Intensity;
            }

            private static readonly Color TintColor = new Color(0, 0.7529412f, 1);
        }
    }
}