using CustomBanners.Configuration;
using CustomBanners.Graphics;
using CustomBanners.Loaders;
using UnityEngine;

namespace CustomBanners
{
    internal class Banner : IGraphicListener
    {
        public GameObject GameObject { get; }

        public Renderer Renderer { get; }

        public Transform Transform { get; }

        public Material Material { get; }

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
        private IGraphic _graphic;

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

        public void SetMedia(BannerMedia media, bool handleColored = true)
        {
            Graphic = media.Graphic;
            if(handleColored && media.Colored) GlowEnabled = ShouldTint = false;
        }

        private static readonly Color TintColor = new Color(0, 0.7529412f, 1);
    }
}