using UnityEngine;

namespace CustomBanners.Animations
{
    public struct ProcessedAnimation
    {
        public readonly Texture2D[] textures;
        public readonly float[] delays;

        public ProcessedAnimation(Texture2D[] textures, float[] delays)
        {
            this.textures = textures;
            this.delays = delays;
        }
    }
}