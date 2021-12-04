using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class SettingsViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand LogOutCommand { get; }
        #endregion

        #region Properties
        public Settings Settings { get; }
        public List<LanguageName> InterfaceLanguages { get; }

        private LanguageName _selectedInterfaceLanguage;
        public LanguageName SelectedInterfaceLanguage
        {
            get
            {
                return _selectedInterfaceLanguage;
            }
            set
            {
                _selectedInterfaceLanguage = value;
                OnPropertyChanged();

                ResourceDictionary resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri("..\\..\\Resources\\Languages\\pl-PL.xaml", UriKind.Relative);

                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
        #endregion

        public SettingsViewModel()
        {
            LogOutCommand = new RelayCommand(LogOut);

            Settings = Settings.GetInstance();
            InterfaceLanguages = new List<LanguageName>();
            InterfaceLanguages.Add(LanguagesService.GetInstance().LanguageNames.First(x => x.SourceLanguageId == 1 && x.LanguageId == 1));
            InterfaceLanguages.Add(LanguagesService.GetInstance().LanguageNames.First(x => x.SourceLanguageId == 2 && x.LanguageId == 2));
            _selectedInterfaceLanguage = LanguagesService.GetInstance().LanguageNames.First(x => x.SourceLanguageId == Settings.InterfaceLanguageId && x.LanguageId == Settings.InterfaceLanguageId);
        }

        private void LogOut(object obj)
        {
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
}
