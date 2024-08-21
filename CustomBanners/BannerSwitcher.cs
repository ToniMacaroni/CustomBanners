using System.Collections;
using UnityEngine;

namespace CustomBanners
{
    internal class BannerSwitcher
    {
        public bool IsSwitching { get; set; }


        private readonly Banner _banner;
        private readonly WaitForSeconds _wait = new WaitForSeconds(0.01f);

        private BannerMedia _queued;
        private Coroutine _runningCoroutine;

        public BannerSwitcher(Banner banner)
        {
            _banner = banner;
        }

        public void SwitchTo(BannerMedia media, ETransitionSpeed speed)
        {
            if (IsSwitching)
            {
                _queued = media;
                return;
            }

            _banner.Transition = -0.01f;
            _banner.Graphic2 = media.Graphic;

            var steps = speed == ETransitionSpeed.Slow ? 0.005f : 0.015f;

            _runningCoroutine = SharedCoroutineStarter.instance.StartCoroutine(Switch(media, steps));
        }

        public IEnumerator Switch(BannerMedia media, float steps)
        {
            IsSwitching = true;

            var transition = 0f;
            while (transition < 1)
            {
                transition += steps;
                _banner.Transition = transition;
                yield return _wait;
            }

            _banner.Transition = -0.01f;

            SwitchGraphics();

            if (media.Colored) _banner.GlowEnabled = _banner.ShouldTint = false;

            if (_queued is { })
            {
                var queued = _queued;
                _queued = null;
                _banner.Transition = -0.01f;
                _banner.Graphic2 = queued.Graphic;
                yield return Switch(queued, steps);
            }

            IsSwitching = false;
        }

        private void SwitchGraphics()
        {
            (_banner.Graphic1, _banner.Graphic2) = (_banner.Graphic2, null);
        }

        public enum ETransitionSpeed
        {
            Slow,
            Fast
        }
    }
}
