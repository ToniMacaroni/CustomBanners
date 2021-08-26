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
using UnityEngine;

namespace CustomBanners.Loaders
{
    internal class ImageLoader : IDisposable
    {
        public bool IsLoaded { get; private set; }

        public Dictionary<string, BannerMedia> Images;

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

            Images = new Dictionary<string, BannerMedia>();
        }

        /// <summary>
        /// Get all medias with the Random flag
        /// </summary>
        /// <returns></returns>
        public List<BannerMedia> GetRandoms()
        {
            return Images.Values.Where(x => x.Random).ToList();
        }

        public string GetRelativePath(string fullPath)
        {
            var dirName = _imageDirectory.FullName;
            if (!dirName.EndsWith("\\")) dirName += "\\";
            return fullPath.Replace(dirName, "");
        }

        /// <summary>
        /// Load a texture by it's name
        /// </summary>
        /// <param name="file"></param>
        /// <param name="skipCheck">Skip file exists check</param>
        /// <param name="animated">is the subject an animated image</param>
        public async Task LoadAsync(FileInfo file, bool skipCheck = false, bool animated = false)
        {
            var relName = GetRelativePath(file.FullName);
            if (Images.ContainsKey(relName)) return;

            if (!skipCheck && !file.Exists) return;

            var data = await file.ReadFileDataAsync();
            IGraphic graphic;
            if (animated)
            {
                ProcessedAnimation gif = await AnimationUtilities.ProcessGIF(data, file.Name);
                GIFGraphic gifGraphic = new GIFGraphic(file.Name, gif.textures.FirstOrDefault());
                _bannerAnimationStateUpdater.Register(gifGraphic, gif);
                _loadedAnimations.Add(gif);
                graphic = gifGraphic;
            }
            else
            {
                var tex = CommonExtensions.CreateTexture(data, Path.GetFileNameWithoutExtension(file.Name));
                graphic = new TextureGraphic(tex);
            }

            var bannerMedia = new BannerMedia(graphic, relName);
            bannerMedia.Random = file.Directory?.Name.Equals("random", StringComparison.OrdinalIgnoreCase)??false;

            Images.Add(relName, bannerMedia);
        }

        public async Task LoadAsync(string relativeName, bool skipCheck = false, bool animated = false)
        {
            var fullPath = Path.Combine(_imageDirectory.FullName, relativeName);
            await LoadAsync(new FileInfo(fullPath), skipCheck, animated);
        }


        /// <summary>
        /// Loads all textures from the "Images" loader
        /// </summary>
        /// <returns></returns>
        public async Task LoadAllAsync()
        {
            if (IsLoaded) return;

            foreach (var file in _imageDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                if (!HandledExtensions.Contains(file.Extension)) continue;

                // don't load the template
                if (string.Equals(file.Name, "template.png", StringComparison.OrdinalIgnoreCase)) continue;

                await LoadAsync(file, true, file.Extension == ".gif");
            }

            IsLoaded = true;
        }

        public bool TryGetMedia(string name, out BannerMedia media) => Images.TryGetValue(name, out media);

        public void Dispose()
        {
            // Lets destroy all the auto-generated textures for redundancy.
            foreach (var processedAnim in _loadedAnimations)
                foreach (var texture in processedAnim.textures)
                    UnityEngine.Object.Destroy(texture);
        }
    }
}