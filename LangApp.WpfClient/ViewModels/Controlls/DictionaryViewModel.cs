using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System;
using System.Windows.Media.Imaging;

namespace LangApp.WpfClient.ViewModels.Controlls
{
    public class DictionaryViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand SearchValueChangedCommand { get; set; }
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
                SortAscending = SortAscending;
            }
        }

        private bool _sortAscending;
        public bool SortAscending
        {
            get
            {
                return _sortAscending;
            }
            set
            {
                _sortAscending = value;
                OnPropertyChanged();

                if(DictionaryCollectionView.SortDescriptions.Count > 0)
                    DictionaryCollectionView.SortDescriptions.RemoveAt(0);

                if (_sortAscending)
                    DictionaryCollectionView.SortDescriptions.Add(new SortDescription("Key", ListSortDirection.Ascending));
                else
                    DictionaryCollectionView.SortDescriptions.Add(new SortDescription("Key", ListSortDirection.Descending));
            }
        }

        public BitmapImage FirstLanguageFlagSource
        {
            get
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(SelectedDictionary.FirstLanguage.FlagUri, UriKind.Relative);
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
                image.UriSource = new Uri(SelectedDictionary.SecondLanguage.FlagUri, UriKind.Relative);
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

            Dictionaries = new List<BilingualDictionary>();
            GenerateDictionaries();
            SelectedDictionary = Dictionaries[0];
            SortAscending = true;
        }

        private void GenerateDictionaries()
        {
            var translationsLists = TranslationsService.GetInstance().TranslationsLists;

            for(int firstLangIndex = 0; firstLangIndex < translationsLists.Count; firstLangIndex++)
            {
                for (int secondLangIndex = firstLangIndex + 1; secondLangIndex < translationsLists.Count; secondLangIndex++)
                {
                    var firstDicitionary = GetDictionary(firstLangIndex, secondLangIndex);

                    Dictionaries.Add(new BilingualDictionary()
                    {
                        FirstLanguage = translationsLists[firstLangIndex].Language,
                        SecondLanguage = translationsLists[secondLangIndex].Language,
                        Dictionary = firstDicitionary
                    });

                    var secondDictionary = new Dictionary<string, string>();

                    foreach (var pair in firstDicitionary)
                    {
                        secondDictionary.Add(pair.Value, pair.Key);
                    }

                    Dictionaries.Add(new BilingualDictionary()
                    {
                        FirstLanguage = translationsLists[secondLangIndex].Language,
                        SecondLanguage = translationsLists[firstLangIndex].Language,
                        Dictionary = secondDictionary
                    });
                }
            }
        }

        private Dictionary<string, string> GetDictionary(int firstLangIndex, int secondLangIndex)
        {
            var dictionary = new Dictionary<string, string>();
            var translationsLists = TranslationsService.GetInstance().TranslationsLists;

            foreach(var translation in translationsLists[firstLangIndex].Translations)
            {
                Translation secondTranslation = translationsLists[secondLangIndex].Translations.FirstOrDefault(x => x.Word.Id == translation.Word.Id);

                if(secondTranslation != null)
                {
                    dictionary.Add(translation.Value, secondTranslation.Value);
                }
            }

            return dictionary;
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
                    var pair = (KeyValuePair<string, string>)o;

                    if (SearchByFirstLanguage)
                        return pair.Key.ToLowerInvariant().Trim().Contains(_searchedText);
                    else
                        return pair.Value.ToLowerInvariant().Trim().Contains(_searchedText);
                };
            }

            DictionaryCollectionView.Refresh();
        }
    }
}
