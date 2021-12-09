using LangApp.Shared.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.ViewModels;
using LangApp.WpfClient.Views.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        private uint _learnSessionCounter;
        public uint LearnSessionCounter
        {
            get
            {
                return _learnSessionCounter;
            }
            set
            {
                _learnSessionCounter = value;
                OnPropertyChanged();
            }
        }

        private uint _testSessionCounter;
        public uint TestSessionCounter
        {
            get
            {
                return _testSessionCounter;
            }
            set
            {
                _testSessionCounter = value;
                OnPropertyChanged();
            }
        }

        private uint _learnAnswerCounter;
        public uint LearnAnswerCounter
        {
            get
            {
                return _learnAnswerCounter;
            }
            set
            {
                _learnAnswerCounter = value;
                OnPropertyChanged();
            }
        }

        private uint _testAnswerCounter;
        public uint TestAnswerCounter
        {
            get
            {
                return _testAnswerCounter;
            }
            set
            {
                _testAnswerCounter = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _lastLearnSession;
        public DateTime? LastLearnSession
        {
            get
            {
                return _lastLearnSession;
            }
            set
            {
                _lastLearnSession = value;
                OnPropertyChanged();
                OnPropertyChanged("LastLearnSessionInfo");
            }
        }

        public string LastLearnSessionInfo
        {
            get
            {
                return GetTimeInfo(_lastLearnSession);
            }
        }

        private DateTime? _lastTestSession;
        public DateTime? LastTestSession
        {
            get
            {
                return _lastTestSession;
            }
            set
            {
                _lastTestSession = value;
                OnPropertyChanged();
                OnPropertyChanged("LastTestSessionInfo");
            }
        }

        public string LastTestSessionInfo
        {
            get
            {
                return GetTimeInfo(_lastTestSession);
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

        private string GetTimeInfo(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return Application.Current.Resources["not_played"].ToString();
            }

            if (dateTime?.Date == DateTime.Now.Date)
            {
                return Application.Current.Resources["today"].ToString() + dateTime?.ToString(", HH:mm:ss");
            }

            if (dateTime?.Date == DateTime.Now.Date.AddDays(-1))
            {
                return Application.Current.Resources["yesterday"].ToString() + dateTime?.ToString(", HH:mm:ss");
            }

            return dateTime?.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public static async Task RefreshToken()
        {
            var userWithToken = await TokensService.GetUserWithTokenAsync(User.Email, User.Password);
            Token = userWithToken.Token;
        }
    }
}
