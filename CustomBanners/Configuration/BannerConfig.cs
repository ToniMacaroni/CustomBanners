namespace CustomBanners.Configuration
{
    internal class BannerConfig
    {
        public bool IsGlowEnabled { get; set; } = true;

        public bool ShouldTint { get; set; } = true;

        public float WindStrength { get; set; } = 0.5f;

        public bool FlipHorizontal { get; set; } = false;

        public float Intensity { get; set; } = 1;

        public string SelectedTexture { get; set; } = "";
    }
}