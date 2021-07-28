using CustomBanners.Animations;
using CustomBanners.Graphics;
using CustomBanners.Helpers;
using IPA.Utilities;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBanners.Loaders
{
    internal class ImageLoader : IDisposable
    {
        public bool IsLoaded { get; private set; }

        public Dictionary<string, IGraphic> Images;

        private readonly List<ProcessedAnimation> _loadedAnimations = new List<ProcessedAnimation>();
        private readonly BannerAnimationStateUpdater _bannerAnimationStateUpdater;
        private readonly DirectoryInfo _imageDirectory;
        private readonly SiraLog _logger;

        private static readonly List<string> HandledExtensions
            = new List<string> { ".png", ".jpg", ".jpeg", ".gif" };

        private ImageLoader(SiraLog logger, BannerAnimationStateUpdater bannerAnimationStateUpdater)
        {
            _logger = logger;
            _bannerAnimationStateUpdater = bannerAnimationStateUpdater;
            _imageDirectory = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Images"));
            _imageDirectory.Create();

            Images = new Dictionary<string, IGraphic>();
        }

        /// <summary>
        /// Load a texture by it's name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="skipCheck">Skip file exists check</param>
        public async Task LoadAsync(string fileName, bool skipCheck = false, bool animated = false)
        {
            if (Images.ContainsKey(fileName)) return;

            var file = _imageDirectory.File(fileName);
            if (!skipCheck && !file.Exists) return;

            var data = await file.ReadFileDataAsync();
            IGraphic graphic;
            if (animated)
            {
                ProcessedAnimation gif = await AnimationUtilities.ProcessGIF(data, fileName);
                GIFGraphic gifGraphic = new GIFGraphic(fileName, gif.textures.FirstOrDefault());
                _bannerAnimationStateUpdater.Register(gifGraphic, gif);
                _loadedAnimations.Add(gif);
                graphic = gifGraphic;
            }
            else
            {
                var tex = CommonExtensions.CreateTexture(data, fileName);
                graphic = new TextureGraphic(tex);
            }
            Images.Add(fileName, graphic);
        }

        /// <summary>
        /// Loads all textures from the "Images" loader
        /// </summary>
        /// <returns></returns>
        public async Task LoadAllAsync()
        {
            if (IsLoaded) return;

            foreach (var file in _imageDirectory.EnumerateFiles())
            {
                if (!HandledExtensions.Contains(file.Extension)) continue;

                // don't load the template
                if (string.Equals(file.Name, "template.png", StringComparison.OrdinalIgnoreCase)) continue;

                await LoadAsync(file.Name, true, file.Extension == ".gif");
            }

            IsLoaded = true;
        }

        public bool TryGetImage(string name, out IGraphic tex) => Images.TryGetValue(name, out tex);

        public void Dispose()
        {
            // Lets destroy all the auto-generated textures for redundancy.
            foreach (var processedAnim in _loadedAnimations)
                foreach (var texture in processedAnim.textures)
                    UnityEngine.Object.Destroy(texture);
        }
    }
}