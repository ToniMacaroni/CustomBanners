using System.IO;
using CustomBanners.Graphics;
using UnityEngine;

namespace CustomBanners
{
    internal class BannerMedia
    {
        public IGraphic Graphic { get; }

        public string Name => Graphic.Name;

        public string NameWithoutExtension => Path.GetFileNameWithoutExtension(Name);

        public bool Colored { get; }

        public BannerMedia(IGraphic graphic)
        {
            Graphic = graphic;
            Colored = NameWithoutExtension.EndsWith("_Color");
        }

        public string GetDisplayName()
        {
            var name = NameWithoutExtension;
            if (!Colored) return name;
            return NameWithoutExtension.Substring(0, NameWithoutExtension.Length - "_Color".Length);
        }
    }
}