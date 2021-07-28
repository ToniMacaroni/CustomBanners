using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomBanners.Loaders;
using CustomBanners.Helpers;
using HMUI;
using ModestTree;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;
using CustomBanners.Graphics;

namespace CustomBanners.Configuration.UI.Views
{
    [ViewDefinition("CustomBanners.Configuration.UI.Views.ImageListView.bsml")]
    [HotReload(RelativePathToLayout = @"ImageListView")]
    internal class ImageListView : BSMLAutomaticViewController
    {
        public event Action<int> OnBannerSelected; 

        public string SelectedBanner => _selectedBannerIndex==0? "Left Banner" : "Right Banner";

        [UIComponent("image-list")] private readonly CustomListTableData _imageList = null;
        [UIValue("loading")] private bool Loading => !_loaded;
        [UIValue("loaded")] private bool Loaded => _loaded;

        private bool _loaded;
        private SiraLog _logger;
        private PluginConfig _config;
        private ImageLoader _imageLoader;

        private IList<IGraphic> _textures;
        private int _selectedBannerIndex;
        private BannerManager.Banner _selectedBanner;
        private BannerManager _bannerManager;

        [Inject]
        private void Construct(SiraLog logger, PluginConfig config, ImageLoader imageLoader, BannerManager bannerManager)
        {
            _logger = logger;
            _imageLoader = imageLoader;
            _config = config;
            _bannerManager = bannerManager;
        }

        [UIAction("#post-parse")]
        private async void Setup()
        {
            await _imageLoader.LoadAllAsync();

            _loaded = true;
            NotifyPropertyChanged(nameof(Loaded));
            NotifyPropertyChanged(nameof(Loading));

            _textures = _imageLoader.Images.Values.ToList();
            RefreshTextureList();

            SelectBanner(0);
        }

        [UIAction("image-selected")]
        private void OnImageSelected(TableView _, int idx)
        {
            _selectedBanner.Graphic = _textures[idx];
        }

        private void OnLeftBannerSelected()
        {
            SelectBanner(0);
        }

        private void OnRightBannerSelected()
        {
            SelectBanner(1);
        }

        private void SelectBanner(int idx)
        {
            _selectedBannerIndex = idx;
            _selectedBanner = _bannerManager.GetBanner((EBannerType) idx);

            var texIdx = GetTexIndex(_selectedBanner?.Graphic?.Name);
            if (texIdx!=-1)
            {
                _imageList.tableView.SelectCellWithIdx(texIdx);
            }
            else
            {
                _imageList.tableView.ClearSelection();
            }

            NotifyPropertyChanged(nameof(SelectedBanner));
            OnBannerSelected?.Invoke(idx);
        }

        private int GetTexIndex(string texName)
        {
            if (_textures == null || _textures.Count == 0) return -1;
            if (string.IsNullOrEmpty(texName)) return -1;
            for (int i = 0; i < _textures.Count; i++)
            {
                if (_textures[i].Name == texName) return i;
            }

            return -1;
        }

        public void RefreshTextureList()
        {
            FillList(_imageList, _textures);
        }

        private void FillList(CustomListTableData list, IEnumerable<IGraphic> textures)
        {
            var cells = new List<CustomListTableData.CustomCellInfo>();
            foreach (var tex in textures)
            {
                var cell = new CustomListTableData.CustomCellInfo(Path.GetFileNameWithoutExtension(tex.Name), null, Utilities.LoadSpriteFromTexture(tex.Default));
                cells.Add(cell);
            }

            list.data = cells;
            list.tableView.ReloadData();
        }
    }
}