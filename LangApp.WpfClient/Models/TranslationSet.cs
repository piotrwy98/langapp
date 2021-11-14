using LangApp.Shared.Models;
using LangApp.WpfClient.ViewModels;

namespace LangApp.WpfClient.Models
{
    public class TranslationSet : NotifyPropertyChanged
    {
        public Translation FirstLanguageTranslation { get; set; }

        public Translation SecondLanguageTranslation { get; set; }

        private uint? _favouriteWordId;
        public uint? FavouriteWordId
        {
            get
            {
                return _favouriteWordId;
            }
            set
            {
                _favouriteWordId = value;
                OnPropertyChanged();
                OnPropertyChanged("IsFavourite");
            }
        }

        public bool IsFavourite
        {
            get
            {
                return _favouriteWordId != null;
            }
        }
    }
}
