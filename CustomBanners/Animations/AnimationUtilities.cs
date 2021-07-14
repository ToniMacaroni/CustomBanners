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
            AnimationInfo result = null;
            await Coroutines.AsTask(GIFUnityDecoder.Process(data, info => result = info));
            return await ProcessAnimationInfo(result, name);
        }

        private static async Task<ProcessedAnimation> ProcessAnimationInfo(AnimationInfo animationInfo, string name)
        {
            float[] delays = new float[animationInfo.frameCount];
            Texture2D[] textures = new Texture2D[animationInfo.frameCount];
            for (int i = 0; i < animationInfo.frameCount; i++)
            {
                if (animationInfo.frames.Count <= i)
                {
                    while (animationInfo.frames.Count <= i)
                        await Task.Yield();
                }

                FrameInfo currentFrameInfo = animationInfo.frames[i];
                delays[i] = currentFrameInfo.delay;

                Texture2D frameTexture = new Texture2D(currentFrameInfo.width, currentFrameInfo.height, TextureFormat.RGBA32, false);
                try
                {
                    frameTexture.name = name;
                    if (i != 0)
                        frameTexture.name += $" ({i})";
                    frameTexture.SetPixels32(currentFrameInfo.colors);
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