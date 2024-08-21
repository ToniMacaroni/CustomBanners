using UnityEngine;

namespace CustomBanners.Graphics
{
    internal interface IGraphic
    {
        Texture2D Default { get; }
        Texture2D Graphic { get; }
        string Name { get; }

        public void AddListener(IGraphicListener listener);
        public void RemoveListener(IGraphicListener listener);
    }
}