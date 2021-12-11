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
using System.Threading.Tasks;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class DictionaryViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand SearchValueChangedCommand { get; }
        public ICommand StarMouseLeftButtonDownCommand { get; }
        public ICommand FirstVolumeMouseLeftButtonDownCommand { get; }
        public ICommand SecondVolumeMouseLeftButtonDownCommand { get; }
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
        #endregion

        #region Variables
        private string _searchedText;
        #endregion

        public DictionaryViewModel()
        {
            SearchValueChangedCommand = new RelayCommand(SearchValueChanged);
            StarMouseLeftButtonDownCommand = new RelayCommand(StarMouseLeftButtonDown);
            FirstVolumeMouseLeftButtonDownCommand = new RelayCommand(FirstVolumeMouseLeftButtonDown);
            SecondVolumeMouseLeftButtonDownCommand = new RelayCommand(SecondVolumeMouseLeftButtonDown);

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

        private async void FirstVolumeMouseLeftButtonDown(object obj)
        {
            var translationSet = obj as TranslationSet;
            if(translationSet != null && !translationSet.IsFirstPlaying)
            {
                translationSet.IsFirstPlaying = true;
                await Task.Run(() => PronunciationsService.GetInstance().PlayPronunciation(translationSet.FirstLanguageTranslation));
                translationSet.IsFirstPlaying = false;
            }
        }

        private async void SecondVolumeMouseLeftButtonDown(object obj)
        {
            var translationSet = obj as TranslationSet;
            if (translationSet != null && !translationSet.IsSecondPlaying)
            {
                translationSet.IsSecondPlaying = true;
                await Task.Run(() => PronunciationsService.GetInstance().PlayPronunciation(translationSet.SecondLanguageTranslation));
                translationSet.IsSecondPlaying = false;
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
