using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class FavouriteWordsViewModel
    {
        public ICommand StarMouseLeftButtonDownCommand { get; set; }

        public static ObservableCollection<TranslationSet> TranslationSets { get; set; }

        public FavouriteWordsViewModel()
        {
            TranslationSets = new ObservableCollection<TranslationSet>();
            var dictionaries = TranslationsService.GetInstance().Dictionaries;

            for (int i=0; i<dictionaries.Count; i=i+2)
            {
                foreach(var pair in dictionaries[i].Dictionary)
                {
                    if(pair.Value.IsFavourite)
                    {
                        TranslationSets.Add(pair.Value);
                    }
                }
            }

            StarMouseLeftButtonDownCommand = new RelayCommand(StarMouseLeftButtonDown);
        }

        private async void StarMouseLeftButtonDown(object obj)
        {
            var favouriteWordId = obj as uint?;
            if (favouriteWordId != null)
            {
                await FavouriteWordsService.RemoveFavouriteWordAsync(favouriteWordId.Value);
            }
        }
    }
}
