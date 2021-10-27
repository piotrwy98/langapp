﻿using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Views.Controls;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Windows
{
    public class MainViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand MainScreenCheckedCommand { get; set; }
        public ICommand LearnCheckedCommand { get; set; }
        public ICommand DictionaryCheckedCommand { get; set; }
        #endregion

        #region Properties
        public Configuration Configuration { get; set; } = Configuration.GetInstance();
        #endregion

        #region Controls
        private MainScreenControl _mainScreenControl = new MainScreenControl();
        private DictionaryControl _dictionaryControl = new DictionaryControl();
        #endregion

        public MainViewModel()
        {
            MainScreenCheckedCommand = new RelayCommand(MainScreenChecked);
            LearnCheckedCommand = new RelayCommand(LearnChecked);
            DictionaryCheckedCommand = new RelayCommand(DictionaryChecked);

            MainScreenChecked();
        }

        private void MainScreenChecked(object obj = null)
        {
            Configuration.CurrentView = _mainScreenControl;
        }

        private void LearnChecked(object obj = null)
        {
            Configuration.CurrentView = Configuration.LearnSettingsControl;
        }

        private void DictionaryChecked(object obj = null)
        {
            Configuration.CurrentView = _dictionaryControl;
        }
    }
}
