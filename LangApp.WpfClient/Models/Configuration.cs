using LangApp.Shared.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.ViewModels;
using LangApp.WpfClient.Views.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows;
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

        public Schedule CurrentSchedule { get; set; }

        private bool _isLearnChecked;
        public bool IsLearnChecked
        {
            get
            {
                return _isLearnChecked;
            }
            set
            {
                _isLearnChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isTestChecked;
        public bool IsTestChecked
        {
            get
            {
                return _isTestChecked;
            }
            set
            {
                _isTestChecked = value;
                OnPropertyChanged();
            }
        }
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

        public static void ArrangeSchedule(Schedule schedule)
        {
            var language = LanguagesService.GetInstance().Languages.First(x => x.Id == schedule.SessionSettings.LanguageId);

            new ToastContentBuilder()
                .SetToastScenario(ToastScenario.Reminder)
                .AddArgument("id", schedule.Id.ToString())
                .AddText(schedule.SessionType == SessionType.LEARN ? Application.Current.Resources["time_to_learn"].ToString() :
                    Application.Current.Resources["time_to_test"].ToString())
                .AddText(Application.Current.Resources["language_colon"].ToString() + " " + schedule.SessionSettings.LanguageInfo)
                .AddText(Application.Current.Resources["categories_colon"].ToString() + " " + schedule.SessionSettings.CategoriesInfo)
                .AddToastInput(new ToastSelectionBox("snoozeTime")
                {
                    DefaultSelectionBoxItemId = "5",
                    Items =
                    {
                                new ToastSelectionBoxItem("1", "1 " + Application.Current.Resources["minute"].ToString()),
                                new ToastSelectionBoxItem("5", "5 " + Application.Current.Resources["minutes"].ToString()),
                                new ToastSelectionBoxItem("15", "15 " + Application.Current.Resources["minutes"].ToString()),
                                new ToastSelectionBoxItem("30", "30 " + Application.Current.Resources["minutes"].ToString()),
                                new ToastSelectionBoxItem("60", "1 " + Application.Current.Resources["hour"].ToString())
                    }
                })

                .AddButton(new ToastButtonSnooze(Application.Current.Resources["snooze"].ToString()) { SelectionBoxId = "snoozeTime" })
                .AddButton(new ToastButton().SetContent(Application.Current.Resources["start"].ToString()))
                .AddButton(new ToastButtonDismiss(Application.Current.Resources["dismiss"].ToString()))
                .Schedule(DateTime.Now.AddMinutes(schedule.IntervalMinutes), toast =>
                {
                    toast.Tag = schedule.Id.ToString();
                });
        }
    }
}
