using System;
using UnityEngine;

namespace CustomBanners.Animations
{

    // Borrowed mostly from ImageFactory which is mostly borrowed from BSML
    internal class RendererAnimationControllerData
    {
        private int _uvIndex;
        private readonly float[] _delays;
        private readonly Texture2D[] _textures;
        private readonly bool _isDelayConsistent = true;
        public DateTime lastSwitch = DateTime.UtcNow;

        public RendererAnimationControllerData(Texture2D[] textures, float[] delays)
        {
            _delays = delays;
            _textures = textures;
            float firstDelay = -1;
            for (int i = 0; i < delays.Length; i++)
            {
                if (i == 0)
                    firstDelay = delays[i];

                if (delays[i] != firstDelay)
                    _isDelayConsistent = false;
            }
        }

        internal Texture2D CheckFrame(DateTime now)
        {
            double differenceMs = (now - lastSwitch).TotalMilliseconds;
            if (differenceMs < _delays[_uvIndex])
                return null;

            if (_isDelayConsistent && _delays[_uvIndex] <= 10 && differenceMs < 100)
            {
                // Bump animations with consistently 10ms or lower frame timings to 100ms
                return null;
            }

            lastSwitch = now;
            do
            {
                _uvIndex++;
                if (_uvIndex >= _textures.Length)
                    _uvIndex = 0;
            }
            while (!_isDelayConsistent && _delays[_uvIndex] == 0);
            return _textures[_uvIndex];
        }
    }
}
