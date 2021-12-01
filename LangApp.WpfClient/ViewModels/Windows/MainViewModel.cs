using LangApp.WpfClient.Models;
using LangApp.WpfClient.ViewModels.Controls;
using LangApp.WpfClient.Views.Controls;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Windows
{
    public class MainViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand MainScreenCheckedCommand { get; set; }
        public ICommand LearnCheckedCommand { get; set; }
        public ICommand TestCheckedCommand { get; set; }
        public ICommand StatsCheckedCommand { get; set; }
        public ICommand DictionaryCheckedCommand { get; set; }
        public ICommand FavouriteWordsCheckedCommand { get; set; }
        #endregion

        #region Properties
        public Configuration Configuration { get; set; } = Configuration.GetInstance();
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
                Configuration.CurrentView = Configuration.LearnSettingsControl;
            }
        }

        private void TestChecked(object obj = null)
        {
            if (Configuration.IsNotDuringTest)
            {
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
    }
}
