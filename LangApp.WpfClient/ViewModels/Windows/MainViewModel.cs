using LangApp.WpfClient.Models;
using LangApp.WpfClient.Views.Controls;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Windows
{
    public class MainViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand MainScreenCheckedCommand { get; }
        public ICommand LearnCheckedCommand { get; }
        public ICommand TestCheckedCommand { get; }
        public ICommand StatsCheckedCommand { get; }
        public ICommand DictionaryCheckedCommand { get; }
        public ICommand FavouriteWordsCheckedCommand { get; }
        public ICommand SettingsCheckedCommand { get; }
        #endregion

        #region Properties
        public Configuration Configuration { get; } = Configuration.GetInstance();
        #endregion

        #region Controls
        private MainScreenControl _mainScreenControl = new MainScreenControl();
        private DictionaryControl _dictionaryControl = new DictionaryControl();
        private FavouriteWordsControl _favouriteWordsControl = new FavouriteWordsControl();
        #endregion

        public MainViewModel()
        {
            MainScreenCheckedCommand = new RelayCommand(MainScreenChecked);
            LearnCheckedCommand = new RelayCommand(LearnChecked);
            TestCheckedCommand = new RelayCommand(TestChecked);
            StatsCheckedCommand = new RelayCommand(StatsChecked);
            DictionaryCheckedCommand = new RelayCommand(DictionaryChecked);
            FavouriteWordsCheckedCommand = new RelayCommand(FavouriteWordsChecked);
            SettingsCheckedCommand = new RelayCommand(SettingsChecked);

            MainScreenChecked();
        }

        private void MainScreenChecked(object obj = null)
        {
            Configuration.CurrentView = _mainScreenControl;
        }

        private void LearnChecked(object obj = null)
        {
            if (Configuration.LearnControl != null)
            {
                Configuration.CurrentView = Configuration.LearnControl;
            }
            else
            {
                var dataContext = Configuration.LearnSettingsControl.DataContext;
                Configuration.LearnSettingsControl.DataContext = null;
                Configuration.LearnSettingsControl.DataContext = dataContext;

                Configuration.CurrentView = Configuration.LearnSettingsControl;
            }
        }

        private void TestChecked(object obj = null)
        {
            if (Configuration.IsNotDuringTest)
            {
                var dataContext = Configuration.TestSettingsControl.DataContext;
                Configuration.TestSettingsControl.DataContext = null;
                Configuration.TestSettingsControl.DataContext = dataContext;

                Configuration.CurrentView = Configuration.TestSettingsControl;
            }
        }

        private void StatsChecked(object obj = null)
        {
            Configuration.StatsControl = new StatsControl();
            Configuration.CurrentView = Configuration.StatsControl;
        }

        private void DictionaryChecked(object obj = null)
        {
            Configuration.CurrentView = _dictionaryControl;
        }

        private void FavouriteWordsChecked(object obj)
        {
            Configuration.CurrentView = _favouriteWordsControl;
        }

        private void SettingsChecked(object obj)
        {
            Configuration.CurrentView = Configuration.SettingsControl;
        }
    }
}
