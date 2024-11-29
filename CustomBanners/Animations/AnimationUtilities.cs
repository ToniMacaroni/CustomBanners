using BeatSaberMarkupLanguage.Animations;
using IPA.Utilities.Async;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomBanners.Animations
{
    internal class AnimationUtilities
    {
        public static async Task<ProcessedAnimation> ProcessGIF(byte[] data, string name)
        {
            AnimationInfo result = await GIFUnityDecoder.ProcessAsync(data);
            return await ProcessAnimationInfo(result, name);
        }

        private static async Task<ProcessedAnimation> ProcessAnimationInfo(AnimationInfo animationInfo, string name)
        {
            float[] delays = new float[animationInfo.Frames.Count];
            Texture2D[] textures = new Texture2D[animationInfo.Frames.Count];
            for (int i = 0; i < animationInfo.Frames.Count; i++)
            {
                if (animationInfo.Frames.Count <= i)
                {
                    while (animationInfo.Frames.Count <= i)
                        await Task.Yield();
                }

                FrameInfo currentFrameInfo = animationInfo.Frames[i];
                delays[i] = currentFrameInfo.Delay;

                Texture2D frameTexture = new Texture2D(currentFrameInfo.Width, currentFrameInfo.Height, TextureFormat.BGRA32, false);
                try
                {
                    frameTexture.name = name;
                    if (i != 0)
                        frameTexture.name += $" ({i})";
                    frameTexture.LoadRawTextureData(currentFrameInfo.Colors);
                    frameTexture.Apply();
                }
                catch
                {
                    await Task.Yield();
                }
                frameTexture.wrapMode = TextureWrapMode.Repeat;
                frameTexture.filterMode = FilterMode.Trilinear;
                textures[i] = frameTexture;
            }
            return new ProcessedAnimation(textures, delays);
        }
    }
}