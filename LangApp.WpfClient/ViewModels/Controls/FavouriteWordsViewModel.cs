using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class FavouriteWordsViewModel
    {
        public ICommand StarMouseLeftButtonDownCommand { get; }

        public static ObservableCollection<FavouriteWord> FavouriteWords { get; set; }

        public FavouriteWordsViewModel()
        {
            StarMouseLeftButtonDownCommand = new RelayCommand(StarMouseLeftButtonDown);

            FavouriteWords = FavouriteWordsService.GetInstance().FavouriteWords;
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
