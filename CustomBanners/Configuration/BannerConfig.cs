namespace CustomBanners.Configuration
{
    internal class BannerConfig
    {
        public bool IsGlowEnabled { get; set; } = true;

        public bool ShouldTint { get; set; } = true;

        public bool RandomFluctuationEnabled { get; set; } = false;

        public string SelectedTexture { get; set; } = "";
    }
}