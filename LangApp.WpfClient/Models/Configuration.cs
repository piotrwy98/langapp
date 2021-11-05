using LangApp.Shared.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Controls;
using System.Windows.Controls;

namespace LangApp.WpfClient.Models
{
    public class Configuration : NotifyPropertyChanged
    {
        private static Configuration _instance;

        private Configuration() { }

        public static Configuration GetInstance()
        {
            if (_instance == null)
                _instance = new Configuration();

            return _instance;
        }

        #region Properties
        private string _token;
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                HttpClientService.SetToken(value);
            }
        }

        public User User { get; set; }

        private UserControl _currentView;
        public UserControl CurrentView
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

        public LearnSettingsControl LearnSettingsControl { get; } = new LearnSettingsControl(false);

        public LearnSettingsControl TestSettingsControl { get; } = new LearnSettingsControl(true);

        public LearnControl LearnControl { get; set; }

        private LearnControl _testControl;
        public LearnControl TestControl
        {
            get
            {
                return _testControl;
            }
            set
            {
                _testControl = value;
                OnPropertyChanged("IsNotDuringTest");
            }
        }

        public bool IsNotDuringTest
        {
            get
            {
                return _testControl == null;
            }
        }

        public LearnFinishControl LearnFinishControl { get; set; }
        #endregion
    }
}
