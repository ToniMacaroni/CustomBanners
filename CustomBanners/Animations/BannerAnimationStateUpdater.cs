using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CustomBanners.Animations
{
    // Mostly borrowed from ImageFactory's SimpleAnimationStateUpdater which is mostly borrowed from BSML's AnimationController
    internal class BannerAnimationStateUpdater : ITickable
    {
        private readonly Dictionary<GIFGraphic, RendererAnimationControllerData> _registeredAnimations = new Dictionary<GIFGraphic, RendererAnimationControllerData>();

        public RendererAnimationControllerData Register(GIFGraphic graphic, ProcessedAnimation processData)
        {
            if (!_registeredAnimations.TryGetValue(graphic, out RendererAnimationControllerData animationData))
            {
                // Add the new data to our registration.
                animationData = new RendererAnimationControllerData(processData.textures, processData.delays);
                _registeredAnimations.Add(graphic, animationData);
            }
            else
            {
                // Destroy the extra textures to free up RAM if
                // a synchronization issue occurs.
                for (int i = 0; i < processData.textures.Length; i++)
                    UnityEngine.Object.Destroy(processData.textures[i]);
            }
            return animationData;
        }

        public void Unregister(GIFGraphic graphic)
        {
            if (_registeredAnimations.ContainsKey(graphic))
            {
                _registeredAnimations.Remove(graphic);
            }
        }

        public virtual void Tick()
        {
            DateTime now = DateTime.UtcNow;

            // For every animation controller we have, update the images under its effect.
            foreach (var anims in _registeredAnimations)
            {
                if (!anims.Key.HasListeners)
                    continue;

                Texture2D texture = anims.Value.CheckFrame(now);
                // If there was a texture update...
                if (texture != null)
                {
                    // Apply it.
                    anims.Key.SetGraphic(texture);
                }
            }
        }
    }
}