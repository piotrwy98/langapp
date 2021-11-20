using LangApp.WpfClient.ViewModels;

namespace LangApp.WpfClient.Models
{
    public class ObjectToChoose : NotifyPropertyChanged
    {
        public object Object { get; set; }

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
