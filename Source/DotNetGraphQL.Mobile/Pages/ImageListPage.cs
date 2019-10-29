﻿using System.Collections.Generic;
using System.Linq;
using DotNetGraphQL.Common;
using Xamarin.Forms;

namespace DotNetGraphQL.Mobile
{
    class DogImageListPage : BaseContentPage<DogImageListViewModel>
    {
        readonly RefreshView _refreshView;

        public DogImageListPage()
        {
            ViewModel.PullToRefreshFailed += HandlePullToRefreshFailed;

            var collectionView = new CollectionView
            {
                ItemTemplate = new DogImageListDataTemplateSelector(),
                SelectionMode = SelectionMode.Single,
                EmptyView = new Label
                {
                    Text = "🐶",
                    FontSize = 128,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                }
            };
            collectionView.SelectionChanged += HandleCollectionViewCollectionChanged;
            collectionView.SetBinding(CollectionView.ItemsSourceProperty, nameof(DogImageListViewModel.DogImageCollection));

            _refreshView = new RefreshView
            {
                Content = collectionView,
                RefreshColor = Color.Red
            };
            _refreshView.SetBinding(RefreshView.IsRefreshingProperty, nameof(DogImageListViewModel.IsDogImageCollectionRefreshing));
            _refreshView.SetBinding(RefreshView.CommandProperty, nameof(DogImageListViewModel.RefreshDogCollectionCommand));

            Title = "Dogs";

            Content = _refreshView;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_refreshView.Content is CollectionView collectionView
                && collectionView.ItemsSource is ICollection<DogImagesModel> dogImagesCollection
                && !dogImagesCollection.Any())
            {
                _refreshView.IsRefreshing = true;
            }
        }

        void HandlePullToRefreshFailed(object sender, string e) =>
            Device.BeginInvokeOnMainThread(() => DisplayAlert("Refresh Failed", e, "OK"));

        async void HandleCollectionViewCollectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var collectionView = (CollectionView)sender;
            collectionView.SelectedItem = null;

            if (e.CurrentSelection.FirstOrDefault() is DogImagesModel dogImagesModel)
            {
                //ToDo Navigate to Dog Images page
                await OpenBrowser(dogImagesModel.AvatarUrl);
            }
        }
    }
}