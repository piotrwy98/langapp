using LangApp.WpfClient.ViewModels;

namespace LangApp.WpfClient.Models
{
    public class ObjectToChoose : NotifyPropertyChanged
    {
        private object _object;
        public object Object
        {
            get
            {
                return _object;
            }
            set
            {
                _object = value;
                OnPropertyChanged();
            }
        }

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
