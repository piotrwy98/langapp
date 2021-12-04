using LangApp.Shared.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.ViewModels;
using LangApp.WpfClient.Views.Controls;
using Newtonsoft.Json;
using System;
using System.Windows.Controls;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Models
{
    public class Configuration : NotifyPropertyChanged
    {
        private static Configuration _instance;

        private static string _token;
        public static string Token
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

        public static User User { get; set; }

        #region Properties
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

        public LearnSettingsControl LearnSettingsControl { get; } = new LearnSettingsControl(SessionType.LEARN);

        public LearnSettingsControl TestSettingsControl { get; } = new LearnSettingsControl(SessionType.TEST);

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

        public StatsControl StatsControl { get; set; }

        public SettingsControl SettingsControl { get; } = new SettingsControl();
        #endregion

        private Configuration()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
        }

        public static Configuration GetInstance()
        {
            if (_instance == null)
                _instance = new Configuration();

            return _instance;
        }
    }
}
