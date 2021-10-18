using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Views.Controlls;
using System;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Windows
{
    public class MainViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand MainScreenCheckedCommand { get; set; }
        public ICommand DictionaryCheckedCommand { get; set; }
        #endregion

        #region Properties
        private object _currentView;
        public object CurrentView
        {
            get
            {
                return _currentView;
            }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Controls
        private MainScreenControl _mainScreenControl = new MainScreenControl();
        private DictionaryControl _dictionaryControl = new DictionaryControl();
        #endregion

        public MainViewModel()
        {
            MainScreenCheckedCommand = new RelayCommand(MainScreenChecked);
            DictionaryCheckedCommand = new RelayCommand(DictionaryChecked);

            MainScreenChecked();
        }

        private void MainScreenChecked(object obj = null)
        {
            CurrentView = _mainScreenControl;
        }

        private void DictionaryChecked(object obj = null)
        {
            CurrentView = _dictionaryControl;
        }
    }
}
