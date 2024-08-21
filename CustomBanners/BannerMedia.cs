using System;
using CustomBanners.Graphics;

namespace CustomBanners
{
    internal class BannerMedia
    {
        public IGraphic Graphic { get; }

        public string Name => Graphic.Name;

        public string RelativeName { get; }

        public bool Colored { get; }

        public bool Random { get; set; }

        public BannerMedia(IGraphic graphic, string relativeName)
        {
            Graphic = graphic;
            RelativeName = relativeName;
            Colored = Name.EndsWith("_Color", StringComparison.OrdinalIgnoreCase);
        }

        public string GetDisplayName()
        {
            if (!Colored) return Name;
            return Name.Substring(0, Name.Length - "_Color".Length);
        }
    }
}
