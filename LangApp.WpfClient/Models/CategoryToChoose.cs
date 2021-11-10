using LangApp.Shared.Models;
using LangApp.WpfClient.ViewModels;

namespace LangApp.WpfClient.Models
{
    public class CategoryToChoose : NotifyPropertyChanged
    {
        public Category Category { get; set; }

        private bool _isChosen;
        public bool IsChosen
        {
            get
            {
                return _isChosen;
            }
            set
            {
                _isChosen = value;
                OnPropertyChanged();
            }
        }
    }
}
