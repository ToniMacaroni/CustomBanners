using CustomBanners.Configuration;
using CustomBanners.Graphics;
using CustomBanners.Loaders;
using TMPro;
using UnityEngine;

namespace CustomBanners
{
    internal class Banner
    {
        public GameObject GameObject { get; }

        public Renderer Renderer { get; }

        public Transform Transform { get; }

        public Material Material { get; }

        public IGraphic Graphic1
        {
            get => _graphic1;
            set
            {
                if (_graphic1 != null)
                    _graphic1.RemoveListener(_tex1UpdateUnit);
                _graphic1 = value;
                if (_graphic1 != null)
                {
                    _graphic1.AddListener(_tex1UpdateUnit);
                    _tex1UpdateUnit.UpdateTexture(_graphic1.Graphic);
                }
            }
        }

        public IGraphic Graphic2
        {
            get => _graphic2;
            set
            {
                if (_graphic2 != null)
                    _graphic2.RemoveListener(_tex2UpdateUnit);
                _graphic2 = value;
                if (_graphic2 != null)
                {
                    _graphic2.AddListener(_tex2UpdateUnit);
                    _tex2UpdateUnit.UpdateTexture(_graphic2.Graphic);
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
                Material?.SetColor(ColorId, value ? _config.Tint : Color.white);
            }
        }

        public Color Tint
        {
            get => _config.Tint;
            set
            {
                _config.Tint = value;
                if (!_config.ShouldTint) return;
                Material?.SetColor(ColorId, value);
            }
        }

        public bool FlipHorizontal
        {
            get => _config.FlipHorizontal;
            set
            {
                _config.FlipHorizontal = value;
                Material?.SetTextureScale(Tex1Id, value ? new Vector2(-1, 1) : Vector2.one);
            }
        }

        public float Intensity
        {
            get => _config.Intensity;
            set
            {
                _config.Intensity = value;
                Material?.SetFloat(IntensityId, value);
            }
        }

        public float Transition
        {
            get => Material?.GetFloat(TransitionId)??0;
            set => Material?.SetFloat(TransitionId, value);
        }

        public float WindStrength
        {
            get => _windStrength;
            set
            {
                _windStrength = value;
                _cloth.externalAcceleration = new Vector3(0, 0, -value);
            }
        }

        public bool ClothActive
        {
            get => _cloth.enabled;
            set => _cloth.enabled = value;
        }

        public bool HighlighterActive
        {
            get => _highlighter.activeSelf;
            set => _highlighter.SetActive(value);
        }

        private readonly BannerConfig _config;
        private readonly Cloth _cloth;
        private IGraphic _graphic1;
        private IGraphic _graphic2;
        private readonly BannerSwitcher _bannerSwitcher;

        private readonly BannerUpdateUnit _tex1UpdateUnit;
        private readonly BannerUpdateUnit _tex2UpdateUnit;

        private readonly GameObject _highlighter;
        private float _windStrength = 0.5f;

        public Banner(Renderer renderer, BannerConfig config)
        {
            _config = config;
            GameObject = renderer.gameObject;
            Renderer = renderer;
            Transform = GameObject.transform;
            _highlighter = Transform.parent.Find("Highlighter").gameObject;
            _cloth = GameObject.GetComponent<Cloth>();
            Material = renderer.material;

            HighlighterActive = false;
            GlowEnabled = GlowEnabled;
            ShouldTint = ShouldTint;
            Tint = Tint;
            FlipHorizontal = FlipHorizontal;
            Intensity = Intensity;
            Transition = -0.01f;

            _bannerSwitcher = new BannerSwitcher(this);

            _tex1UpdateUnit = new BannerUpdateUnit(Material, Tex1Id);
            _tex2UpdateUnit = new BannerUpdateUnit(Material, Tex2Id);
        }

        public void ResetTint()
        {
            Tint = TintColor;
        }

        public void SetMedia(BannerMedia media, bool handleColored = true)
        {
            Graphic1 = media.Graphic;
            _config.SelectedTexture = media.RelativeName;
            if(handleColored && media.Colored) GlowEnabled = ShouldTint = false;
        }

        public void SetMediaAnimated(BannerMedia media, BannerSwitcher.ETransitionSpeed speed)
        {
            _bannerSwitcher.SwitchTo(media, speed);
            _config.SelectedTexture = media.RelativeName;
        }

        internal class BannerUpdateUnit : IGraphicListener
        {
            private readonly Material _material;
            private readonly int _texId;

            public BannerUpdateUnit(Material material, int texId)
            {
                _material = material;
                _texId = texId;
            }

            public void UpdateTexture(Texture2D newTexture)
            {
                if (_material == null) return;
                _material.SetTexture(_texId, newTexture);
            }
        }

        private static readonly Color TintColor = new Color(0, 0.7529412f, 1);
        private static readonly int Tex1Id = Shader.PropertyToID("_MainTex");
        private static readonly int Tex2Id = Shader.PropertyToID("_SecMainTex");
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int IntensityId = Shader.PropertyToID("_Intensity");
        private static readonly int TransitionId = Shader.PropertyToID("_Transition");
    }
}