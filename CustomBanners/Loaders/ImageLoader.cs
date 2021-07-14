using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CustomBanners;
using CustomBanners.Graphics;
using CustomBanners.Helpers;
using IPA.Utilities;
using IPA.Utilities.Async;
using SiraUtil.Tools;
using UnityEngine;

namespace CustomBanners.Loaders
{
    internal class ImageLoader
    {
        public bool IsLoaded { get; private set; }

        public Dictionary<string, IGraphic> Images;

        private readonly DirectoryInfo _imageDirectory;
        private readonly SiraLog _logger;

        private static readonly List<string> HandledExtensions
            = new List<string> {".png", ".jpg", ".jpeg"};

        private ImageLoader(SiraLog logger)
        {
            _logger = logger;

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
            IGraphic graphic = null;
            if (animated)
            {
                // TODO: Implement animations
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
    }
}