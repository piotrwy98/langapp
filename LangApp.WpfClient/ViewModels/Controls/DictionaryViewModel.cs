using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System;
using System.Windows.Media.Imaging;
using System.Linq;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class DictionaryViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand SearchValueChangedCommand { get; set; }
        public ICommand StarMouseLeftButtonDownCommand { get; set; }
        #endregion

        #region Properties
        public List<BilingualDictionary> Dictionaries { get; }

        private BilingualDictionary _selectedDictionary;
        public BilingualDictionary SelectedDictionary
        {
            get
            {
                return _selectedDictionary;
            }
            set
            {
                _selectedDictionary = value;
                OnPropertyChanged();
                OnPropertyChanged("FirstLanguageFlagSource");
                OnPropertyChanged("SecondLanguageFlagSource");
                DictionaryCollectionView = CollectionViewSource.GetDefaultView(_selectedDictionary.Dictionary);
            }
        }

        private bool _searchByFirstLanguage = true;
        public bool SearchByFirstLanguage
        {
            get
            {
                return _searchByFirstLanguage;
            }
            set
            {
                _searchByFirstLanguage = value;
                OnPropertyChanged();
                RefreshSearching();
            }
        }

        public ICollectionView _dictionaryCollectionView;
        public ICollectionView DictionaryCollectionView
        {
            get
            {
                return _dictionaryCollectionView;
            }
            set
            {
                _dictionaryCollectionView = value;
                OnPropertyChanged();
                RefreshSearching();
            }
        }

        public BitmapImage FirstLanguageFlagSource
        {
            get
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(SelectedDictionary.FirstLanguage.ImagePath, UriKind.Relative);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                return image;
            }
        }

        public BitmapImage SecondLanguageFlagSource
        {
            get
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(SelectedDictionary.SecondLanguage.ImagePath, UriKind.Relative);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                return image;
            }
        }
        #endregion

        #region Variables
        private string _searchedText;
        #endregion

        public DictionaryViewModel()
        {
            SearchValueChangedCommand = new RelayCommand(SearchValueChanged);
            StarMouseLeftButtonDownCommand = new RelayCommand(StarMouseLeftButtonDown);

            Dictionaries = TranslationsService.GetInstance().Dictionaries
                .OrderByDescending(x => x.FirstLanguage.Code).ToList();
            SelectedDictionary = Dictionaries[0];
        }

        private void SearchValueChanged(object obj)
        {
            var args = obj as TextChangedEventArgs;
            if (args != null)
            {
                _searchedText = (args.Source as TextBox).Text.ToLowerInvariant().Trim();
                RefreshSearching();
            }
        }

        private async void StarMouseLeftButtonDown(object obj)
        {
            var pair = obj as KeyValuePair<Word, TranslationSet>?;
            if(pair != null)
            {
                if(pair.Value.Value.IsFavourite)
                {
                    await FavouriteWordsService.RemoveFavouriteWordAsync(pair.Value.Value.FavouriteWordId.Value);
                }
                else
                {
                    await FavouriteWordsService.CreateFavouriteWordAsync
                        (pair.Value.Value.FirstLanguageTranslation.Id,
                        pair.Value.Value.SecondLanguageTranslation.Id);
                }
            }
        }

        private void RefreshSearching()
        {
            if (String.IsNullOrEmpty(_searchedText))
            {
                DictionaryCollectionView.Filter = null;
            }
            else
            {
                DictionaryCollectionView.Filter = o =>
                {
                    var pair = (KeyValuePair<Word, TranslationSet>)o;

                    if (SearchByFirstLanguage)
                        return pair.Value.FirstLanguageTranslation.Value.ToLowerInvariant().Trim().Contains(_searchedText);
                    else
                        return pair.Value.SecondLanguageTranslation.Value.ToLowerInvariant().Trim().Contains(_searchedText);
                };
            }

            DictionaryCollectionView.Refresh();
        }
    }
}
