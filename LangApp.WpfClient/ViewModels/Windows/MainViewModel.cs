using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Controls;
using LangApp.WpfClient.Views.Windows;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.ViewModels.Windows
{
    public class MainViewModel
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

            Configuration.LearnSettingsControl = new LearnSettingsControl(SessionType.LEARN);
            Configuration.TestSettingsControl = new LearnSettingsControl(SessionType.TEST);
            Configuration.DictionaryControl = new DictionaryControl();
            Configuration.FavouriteWordsControl = new FavouriteWordsControl();
            Configuration.SettingsControl = new SettingsControl();

            PartsOfSpeechService.GetInstance();

            PrepareNotifications();
            MainScreenChecked();
        }

        private void PrepareNotifications()
        {
            ToastNotificationManagerCompat.OnActivated += async toastArgs =>
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
                var schedule = Settings.GetInstance().Schedules.FirstOrDefault(x => x.Id.ToString() == args["id"]);

                if (schedule != null)
                {
                    if(args.Contains("dismissed"))
                    {
                        schedule.IsActive = false;
                        Settings.Store();
                    }
                    else
                    {
                        try
                        {
                            // utworzenie sesji
                            var session = await Task.Run(() => SessionsService.CreateSessionAsync(Settings.GetInstance().InterfaceLanguageId, schedule.SessionSettings.LanguageId, schedule.SessionType, schedule.SessionSettings.NumberOfQuestions));

                            if (session != null)
                            {
                                // dodanie wybranych kategori do utworzonej sesji
                                foreach (var id in schedule.SessionSettings.CategoriesIds)
                                {
                                    await Task.Run(() => SelectedCategoriesService.CreateSelectedCategoryAsync(session.Id, id));
                                }

                                // zapisanie w konfiguracji
                                Configuration.CurrentSchedule = schedule;

                                Application.Current.Dispatcher.Invoke(delegate
                                {
                                    if (schedule.SessionType == SessionType.TEST)
                                    {
                                        Configuration.GetInstance().TestControl = new LearnControl(session, schedule.SessionSettings);
                                        Configuration.GetInstance().CurrentView = Configuration.GetInstance().TestControl;
                                        Configuration.GetInstance().IsTestChecked = true;
                                    }
                                    else
                                    {
                                        Configuration.GetInstance().LearnControl = new LearnControl(session, schedule.SessionSettings);
                                        Configuration.GetInstance().CurrentView = Configuration.GetInstance().LearnControl;
                                        Configuration.GetInstance().IsLearnChecked = true;
                                    }

                                    App.EnsureWindowVisibility(Application.Current.Windows[0]);
                                });
                            }
                        }
                        catch
                        {
                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                Application.Current.Windows[0].Show();
                                Application.Current.Windows[0].WindowState = WindowState.Normal;
                                Configuration.GetInstance().NoConnection = true;
                                Configuration.ArrangeSchedule(schedule);
                            });
                        }
                    }
                }
            };

            // uruchomienie aktywnych powiadomień
            foreach (var schedule in Settings.GetInstance().Schedules)
            {
                if (schedule.IsActive && schedule.UserId == Configuration.User.Id)
                {
                    Configuration.ArrangeSchedule(schedule);
                }
            }
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
            Configuration.CurrentView = Configuration.DictionaryControl;
        }

        private void FavouriteWordsChecked(object obj)
        {
            Configuration.CurrentView = Configuration.FavouriteWordsControl;
        }

        private void SettingsChecked(object obj)
        {
            Configuration.CurrentView = Configuration.SettingsControl;
        }
    }
}
