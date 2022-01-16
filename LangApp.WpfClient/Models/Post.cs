using LangApp.Shared.Models;
using LangApp.WpfClient.ViewModels;

namespace LangApp.WpfClient.Models
{
    public class Post : NotifyPropertyChanged
    {
        private News _news;
        public News News
        {
            get
            {
                return _news;
            }
            set
            {
                _news = value;
                OnPropertyChanged();
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get
            {
                return _isEditing;
            }
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get
            {
                return _isSaving;
            }
            set
            {
                _isSaving = value;
                OnPropertyChanged();
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        public Post(News news)
        {
            News = news;
            _title = news.Title;
            _content = news.Content;
        }
    }
}
