using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomBanners.Loaders;
using HMUI;
using Zenject;
using SiraUtil.Logging;

namespace CustomBanners.Configuration.UI.Views
{
    [ViewDefinition("CustomBanners.Configuration.UI.Views.ImageListView.bsml")]
    [HotReload(RelativePathToLayout = @"ImageListView")]
    internal class ImageListView : BSMLAutomaticViewController
    {
        public event Action<int> OnBannerSelected;
        public event Action OnImageChanged;

        public string SelectedBanner => _selectedBannerIndex==0? "Left Banner" : "Right Banner";

        [UIComponent("image-list")] private readonly CustomListTableData _imageList = null;
        [UIValue("loading")] private bool Loading => !_loaded;
        [UIValue("loaded")] private bool Loaded => _loaded;

        private bool _loaded;
        private SiraLog _logger;
        private PluginConfig _config;
        private ImageLoader _imageLoader;

        private IList<BannerMedia> _mediaList;
        private int _selectedBannerIndex;
        private Banner _selectedBanner;
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

            _mediaList = _imageLoader.Images.Values.ToList();
            RefreshTextureList();

            SelectBanner(0);
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (_selectedBanner is { })
            {
                _selectedBanner.HighlighterActive = true;
            }
        }

        [UIAction("image-selected")]
        private void OnImageSelected(TableView _, int idx)
        {
            _selectedBanner.SetMediaAnimated(_mediaList[idx], BannerSwitcher.ETransitionSpeed.Fast);
            OnImageChanged?.Invoke();
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

            if (_selectedBanner is { })
            {
                _selectedBanner.HighlighterActive = false;
            }

            _selectedBanner = _bannerManager.GetBanner((EBannerType) idx);

            if (_selectedBanner is { })
            {
                _selectedBanner.HighlighterActive = true;
            }

            var texIdx = GetTexIndex(_selectedBanner?.Graphic1?.Name);
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
            if (_mediaList == null || _mediaList.Count == 0) return -1;
            if (string.IsNullOrEmpty(texName)) return -1;
            for (int i = 0; i < _mediaList.Count; i++)
            {
                if (_mediaList[i].Graphic.Name == texName) return i;
            }

            return -1;
        }

        public void RefreshTextureList()
        {
            FillList(_imageList, _mediaList);
        }

        private void FillList(CustomListTableData list, IEnumerable<BannerMedia> media)
        {
            var cells = new List<CustomListTableData.CustomCellInfo>();
            foreach (var tex in media)
            {
                var cell = new CustomListTableData.CustomCellInfo(tex.GetDisplayName(), null, Utilities.LoadSpriteFromTexture(tex.Graphic.Default));
                cells.Add(cell);
            }

            list.data = cells;
            list.tableView.ReloadData();
        }
    }
}