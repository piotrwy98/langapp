using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Windows;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.ViewModels.Windows
{
    public class LoginRegisterViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand LogInCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

        public ICommand GoToLogInCommand { get; set; }

        public ICommand GoToRegisterCommand { get; set; }

        public ICommand PasswordChangedCommand { get; set; }

        public ICommand RepeatPasswordChangedCommand { get; set; }
        #endregion

        #region Properties
        public string Email { get; set; }

        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password { get; set; }

        public string RepeatPassword { get; set; }

        private string _errorMessageText;
        public string ErrorMessageText
        {
            get
            {
                return _errorMessageText;
            }
            set
            {
                _errorMessageText = value;
                OnPropertyChanged();
                OnPropertyChanged("ErrorIconVisibility");

                if (!String.IsNullOrEmpty(value))
                    SuccessMessageText = "";
            }
        }

        private string _successMessageText;
        public string SuccessMessageText
        {
            get
            {
                return _successMessageText;
            }
            set
            {
                _successMessageText = value;
                OnPropertyChanged();
                OnPropertyChanged("SuccessIconVisibility");

                if (!String.IsNullOrEmpty(value))
                    ErrorMessageText = "";
            }
        }

        public Visibility _logInVisibility;
        public Visibility LogInVisibility
        {
            get
            {
                return _logInVisibility;
            }
            set
            {
                _logInVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility _registerVisibility = Visibility.Collapsed;
        public Visibility RegisterVisibility
        {
            get
            {
                return _registerVisibility;
            }
            set
            {
                _registerVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ErrorIconVisibility
        {
            get
            {
                if (String.IsNullOrEmpty(ErrorMessageText))
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
        }

        public Visibility SuccessIconVisibility
        {
            get
            {
                if (String.IsNullOrEmpty(SuccessMessageText))
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
        }
        #endregion

        public LoginRegisterViewModel()
        {
            LogInCommand = new RelayCommand(LogIn);
            RegisterCommand = new RelayCommand(Register);
            GoToLogInCommand = new RelayCommand(GoToLogIn);
            GoToRegisterCommand = new RelayCommand(GoToRegister);
            PasswordChangedCommand = new RelayCommand(PasswordChanged);
            RepeatPasswordChangedCommand = new RelayCommand(RepeatPasswordChanged);
        }

        private async void LogIn(object obj = null)
        {
            if (String.IsNullOrEmpty(Email))
            {
                ErrorMessageText = "Nie podano adresu email";
                return;
            }

            if(!new EmailAddressAttribute().IsValid(Email.Trim()))
            {
                ErrorMessageText = "Nieprawidłowy format adresu email";
                return;
            }

            if (String.IsNullOrEmpty(Password))
            {
                ErrorMessageText = "Nie podano hasła";
                return;
            }

            UserWithToken userWithToken;

            try
            {
                userWithToken = await TokensService.GetUserWithTokenAsync(Email.Trim(), Password);
            }
            catch (HttpRequestException)
            {
                ErrorMessageText = "Brak połączenia z serwerem";
                return;
            }

            if(userWithToken != null)
            {
                Configuration.GetInstance().User = userWithToken.User;
                Configuration.GetInstance().Token = userWithToken.Token;

                new MainWindow().Show();
                Application.Current.Windows[0].Close();
            }
            else
            {
                ErrorMessageText = "Błędne dane logowania";
            }
        }

        private async void Register(object obj = null)
        {
            if (String.IsNullOrEmpty(Email))
            {
                ErrorMessageText = "Nie podano adresu email";
                return;
            }

            if (!new EmailAddressAttribute().IsValid(Email.Trim()))
            {
                ErrorMessageText = "Nieprawidłowy format adresu email";
                return;
            }

            if (String.IsNullOrEmpty(Username))
            {
                ErrorMessageText = "Nie podano nazwy użytkownika";
                return;
            }

            if (String.IsNullOrEmpty(Password))
            {
                ErrorMessageText = "Nie podano hasła";
                return;
            }

            if (Password != RepeatPassword)
            {
                ErrorMessageText = "Hasła nie są zgodne";
                return;
            }

            RegisterResult registerResult;

            try
            {
                registerResult = await UsersService.CreateUserAsync(Email.Trim(), Username.Trim(), Password.Trim(), UserRole.USER);

            }
            catch (HttpRequestException)
            {
                ErrorMessageText = "Brak połączenia z serwerem";
                return;
            }

            switch(registerResult)
            {
                case RegisterResult.OK:
                    SuccessMessageText = "Błędne dane logowania";
                    GoToLogIn();
                    break;

                case RegisterResult.OCCUPIED_EMAIL:
                    ErrorMessageText = "Podany adres email jest zajęty";
                    break;

                case RegisterResult.OCCUPIED_USERNAME:
                    ErrorMessageText = "Podana nazwa użytkownika jest zajęta";
                    break;
            }
        }

        private void GoToLogIn(object obj = null)
        {
            LogInVisibility = Visibility.Visible;
            RegisterVisibility = Visibility.Collapsed;
            ErrorMessageText = "";
        }

        private void GoToRegister(object obj)
        {
            LogInVisibility = Visibility.Collapsed;
            RegisterVisibility = Visibility.Visible;
            Username = "";
            ErrorMessageText = "";
        }

        private void PasswordChanged(object obj)
        {
            var passwordBox = obj as PasswordBox;
            if(passwordBox != null)
            {
                Password = passwordBox.Password;
            } 
        }

        private void RepeatPasswordChanged(object obj)
        {
            var passwordBox = obj as PasswordBox;
            if (passwordBox != null)
            {
                RepeatPassword = passwordBox.Password;
            }
        }
    }
}
