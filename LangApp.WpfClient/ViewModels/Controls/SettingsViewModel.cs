using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Controls;
using LangApp.WpfClient.Views.Windows;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class SettingsViewModel : NotifyPropertyChanged
    {
        private static readonly string APP_NAME = "LangApp";

        #region Commands
        public ICommand LogOutCommand { get; }
        public ICommand AddNotificationCommand { get; }
        public ICommand DecrementIntervalCommand { get; }
        public ICommand IncrementIntervalCommand { get; }
        public ICommand RemoveNotificationCommand { get; }
        public ICommand CustomizeNotificationSettingsCommand { get; }
        public ICommand IsActiveClickCommand { get; }
        #endregion

        #region Properties
        public Settings Settings { get; }

        public List<LanguageName> InterfaceLanguages { get; }

        public LanguageName SelectedInterfaceLanguage
        {
            get
            {
                return LanguagesService.GetInstance().LanguageNames.First(x => x.SourceLanguageId == Settings.InterfaceLanguageId && x.LanguageId == Settings.InterfaceLanguageId);
            }
            set
            {
                Settings.InterfaceLanguageId = value.SourceLanguageId;
                Settings.Store();

                Configuration.GetInstance().OnPropertyChanged("LastLearnSessionInfo");
                Configuration.GetInstance().OnPropertyChanged("LastTestSessionInfo");
                Configuration.GetInstance().LearnSettingsControl = new LearnSettingsControl(SessionType.LEARN);
                Configuration.GetInstance().TestSettingsControl = new LearnSettingsControl(SessionType.TEST);
                Configuration.GetInstance().DictionaryControl = new DictionaryControl();
                Configuration.GetInstance().FavouriteWordsControl = new FavouriteWordsControl();

                foreach(var schedule in Settings.Schedules)
                {
                    schedule.SessionSettings.OnPropertyChanged("LanguageInfo");
                    schedule.SessionSettings.OnPropertyChanged("CategoriesInfo");
                    schedule.IsEnabled = schedule.SessionSettings.LanguageId != Settings.GetInstance().InterfaceLanguageId;

                    if(!schedule.IsEnabled)
                    {
                        schedule.IsActive = false;
                        IsActiveClick(schedule);
                    }
                }
            }
        }

        public bool StartWithSystem
        {
            get
            {
                return Settings.StartWithSystem;
            }
            set
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly assembly = Assembly.GetEntryAssembly();

                if (value)
                {
                    key.SetValue(APP_NAME, assembly.Location);
                }
                else
                {
                    key.DeleteValue(APP_NAME);
                }

                Settings.StartWithSystem = value;
                Settings.Store();
            }
        }

        public bool MinimizeToSystemTray
        {
            get
            {
                return Settings.MinimizeToSystemTray;
            }
            set
            {
                Settings.MinimizeToSystemTray = value;
                Settings.Store();
            }
        }

        public Schedule CustomizingSchedule { get; set; }
        #endregion

        public SettingsViewModel()
        {
            LogOutCommand = new RelayCommand(LogOut);
            AddNotificationCommand = new RelayCommand(AddNotification);
            DecrementIntervalCommand = new RelayCommand(DecrementInterval);
            IncrementIntervalCommand = new RelayCommand(IncrementInterval);
            RemoveNotificationCommand = new RelayCommand(RemoveNotification);
            CustomizeNotificationSettingsCommand = new RelayCommand(CustomizeNotificationSettings);
            IsActiveClickCommand = new RelayCommand(IsActiveClick);

            Settings = Settings.GetInstance();
            InterfaceLanguages = new List<LanguageName>();
            InterfaceLanguages.Add(LanguagesService.GetInstance().LanguageNames.First(x => x.SourceLanguageId == 1 && x.LanguageId == 1));
            InterfaceLanguages.Add(LanguagesService.GetInstance().LanguageNames.First(x => x.SourceLanguageId == 2 && x.LanguageId == 2));

            CollectionView view = (CollectionView) CollectionViewSource.GetDefaultView(Settings.Schedules);
            view.Filter = x => (x as Schedule).UserId == Configuration.User.Id;

            foreach (var schedule in Settings.Schedules)
            {
                schedule.IsEnabled = schedule.SessionSettings.LanguageId != Settings.GetInstance().InterfaceLanguageId;

                if (!schedule.IsEnabled)
                {
                    schedule.IsActive = false;
                    IsActiveClick(schedule);
                }
            }
        }

        private void LogOut(object obj)
        {
            var confirmationWindow = new ConfirmationWindow(Application.Current.Resources["log_out"].ToString(),
                Application.Current.Resources["log_out_confirmation"].ToString());
            confirmationWindow.ShowDialog();

            if (confirmationWindow.DialogResult == true)
            {
                AnswersService.NullInstance();
                FavouriteWordsService.NullInstance();
                SelectedCategoriesService.NullInstance();
                SessionsService.NullInstance();
                TranslationsService.NullInstance();

                Configuration.GetInstance().LearnControl = null;
                Configuration.GetInstance().TestControl = null;
                Configuration.GetInstance().CurrentSchedule = null;

                // usunięcie danych z zapisanych ustawień
                Settings.GetInstance().PreviousUserEmail = null;
                Settings.GetInstance().PreviousUserPassword = null;
                Settings.Store();

                // wyświetlenie okna logowania/rejestracji
                var loginRegisterWindow = new LoginRegisterWindow(false);
                loginRegisterWindow.Show();

                Application.Current.Windows[0].Close();
            }
        }

        private void AddNotification(object obj)
        {
            var schedule = new Schedule()
            {
                Id = Guid.NewGuid(),
                SessionSettings = new SessionSettings()
                {
                    NumberOfQuestions = 5,
                    LanguageId = 3,
                    IsClosedChosen = true,
                    CategoriesIds = new List<uint>() { 1 }
                },
                UserId = Configuration.User.Id,
            };

            Settings.Schedules.Add(schedule);
            IsActiveClick(schedule);
        }

        private void DecrementInterval(object obj)
        {
            var schedule = obj as Schedule;
            if(schedule != null)
            {
                schedule.IntervalMinutes--;
                Settings.Store();
            }
        }

        private void IncrementInterval(object obj)
        {
            var schedule = obj as Schedule;
            if (schedule != null)
            {
                schedule.IntervalMinutes++;
                Settings.Store();
            }
        }

        private void RemoveNotification(object obj)
        {
            var schedule = obj as Schedule;
            if (schedule != null)
            {
                var confirmationWindow = new ConfirmationWindow(Application.Current.Resources["remove_notification"].ToString(),
                    Application.Current.Resources["remove_notification_confirmation"].ToString());
                confirmationWindow.ShowDialog();

                if (confirmationWindow.DialogResult == true)
                {
                    schedule.IsActive = false;
                    Settings.Schedules.Remove(schedule);
                    IsActiveClick(schedule);
                }
            }
        }

        private void CustomizeNotificationSettings(object obj)
        {
            var schedule = obj as Schedule;
            if (schedule != null)
            {
                CustomizingSchedule = schedule;

                // kopia głęboka ustawień sesji
                var json = JsonConvert.SerializeObject(schedule.SessionSettings);
                var sessionSettingsCopy = JsonConvert.DeserializeObject<SessionSettings>(json);

                Configuration.GetInstance().CurrentView = new LearnSettingsControl(null, sessionSettingsCopy);
            }
        }

        private void IsActiveClick(object obj)
        {
            var schedule = obj as Schedule;
            if (schedule != null)
            {
                Settings.Store();

                if(schedule.IsActive)
                {
                    Configuration.ArrangeSchedule(schedule);
                }
                else
                {
                    ToastNotificationManagerCompat.History.Remove(schedule.Id.ToString());

                    var notifier = ToastNotificationManagerCompat.CreateToastNotifier();
                    var scheduledToasts = notifier.GetScheduledToastNotifications();
                    var toastToRemove = scheduledToasts.FirstOrDefault(toast => toast.Tag == schedule.Id.ToString());
                    
                    if (toastToRemove != null)
                    {
                        notifier.RemoveFromSchedule(toastToRemove);
                    }
                }
            }
        }
    }
}
