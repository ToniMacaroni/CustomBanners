using UnityEngine;

namespace CustomBanners.Graphics
{
    internal class TextureGraphic : IGraphic
    {
        private readonly Texture2D _texture;
        public string Name => _texture.name;
        public Texture2D Graphic => _texture;
        public Texture2D Default => _texture;

        public TextureGraphic(Texture2D texture)
        {
            _texture = texture;
        }

        public void AddListener(IGraphicListener listener) { /* We don't really wanna do anything. */ }
        public void RemoveListener(IGraphicListener listener) { /* We don't really wanna do anything. */ }
    }
}