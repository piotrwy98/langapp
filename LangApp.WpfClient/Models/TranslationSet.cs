using LangApp.Shared.Models;
using LangApp.WpfClient.ViewModels;

namespace LangApp.WpfClient.Models
{
    public class TranslationSet : NotifyPropertyChanged
    {
        public Translation FirstLanguageTranslation { get; set; }

        public Translation SecondLanguageTranslation { get; set; }

        public uint LearnCount { get; set; }

        public uint TestCount { get; set; }

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

        private bool _isFirstPlaying;
        public bool IsFirstPlaying
        {
            get
            {
                return _isFirstPlaying;
            }
            set
            {
                _isFirstPlaying = value;
                OnPropertyChanged();
            }
        }

        private bool _isSecondPlaying;
        public bool IsSecondPlaying
        {
            get
            {
                return _isSecondPlaying;
            }
            set
            {
                _isSecondPlaying = value;
                OnPropertyChanged();
            }
        }
    }
}
