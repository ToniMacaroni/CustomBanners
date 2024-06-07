using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using UnityEngine;

namespace CustomBanners.Configuration
{
    internal class BannerConfig
    {
        public bool IsGlowEnabled { get; set; } = false;

        public bool ShouldTint { get; set; } = false;

        [UseConverter(typeof(HexColorConverter))]
        public Color Tint { get; set; } = new Color(0, 0.7529412f, 1);

        public bool FlipHorizontal { get; set; } = false;

        public float Intensity { get; set; } = 1;

        public string SelectedTexture { get; set; } = "";
    }
}
