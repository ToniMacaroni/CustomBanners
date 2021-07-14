using UnityEngine;

namespace CustomBanners.Graphics
{
    internal interface IGraphicListener
    {
        void UpdateTexture(Texture2D newTexture);
    }
}