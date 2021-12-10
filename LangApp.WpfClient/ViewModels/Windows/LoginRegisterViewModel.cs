using LangApp.Shared.Models.Controllers;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Windows;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.ViewModels.Windows
{
    public class LoginRegisterViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand LogInCommand { get; }

        public ICommand RegisterCommand { get; }

        public ICommand GoToLogInCommand { get; }

        public ICommand GoToRegisterCommand { get; }

        public ICommand PasswordChangedCommand { get; }

        public ICommand RepeatPasswordChangedCommand { get; }

        public ICommand WindowKeyDownCommand { get; }
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

        private bool _isResultSuccess;
        public bool IsResultSuccess
        {
            get
            {
                return _isResultSuccess;
            }
            set
            {
                _isResultSuccess = value;
                OnPropertyChanged();
            }
        }

        private string _resultMessage;
        public string ResultMessage
        {
            get
            {
                return _resultMessage;
            }
            set
            {
                _resultMessage = value;
                OnPropertyChanged();
                OnPropertyChanged("ResultVisibility");
            }
        }

        private bool _isProcessing;
        public bool IsProcessing
        {
            get
            {
                return _isProcessing;
            }
            set
            {
                _isProcessing = value;
                OnPropertyChanged();
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

        public Visibility ResultVisibility
        {
            get
            {
                if (String.IsNullOrEmpty(_resultMessage))
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
        }
        #endregion

        public LoginRegisterViewModel(bool serverFailed)
        {
            LogInCommand = new RelayCommand(LogIn);
            RegisterCommand = new RelayCommand(Register);
            GoToLogInCommand = new RelayCommand(GoToLogIn);
            GoToRegisterCommand = new RelayCommand(GoToRegister);
            PasswordChangedCommand = new RelayCommand(PasswordChanged);
            RepeatPasswordChangedCommand = new RelayCommand(RepeatPasswordChanged);
            WindowKeyDownCommand = new RelayCommand(WindowKeyDown);

            if(serverFailed)
            {
                ResultMessage = Application.Current.Resources["no_server_connection"].ToString();
            }
        }

        private async void LogIn(object obj = null)
        {
            IsProcessing = true;
            ResultMessage = string.Empty;
            IsResultSuccess = false;

            await CheckLogIn();

            IsProcessing = false;
        }

        private async Task CheckLogIn()
        {
            if (String.IsNullOrEmpty(Email))
            {
                ResultMessage = Application.Current.Resources["no_email_address_provided"].ToString();
                return;
            }

            if (!new EmailAddressAttribute().IsValid(Email.Trim()))
            {
                ResultMessage = Application.Current.Resources["invalid_email_format"].ToString();
                return;
            }

            if (String.IsNullOrEmpty(Password))
            {
                ResultMessage = Application.Current.Resources["no_password_provided"].ToString();
                return;
            }

            UserWithToken userWithToken;

            try
            {
                userWithToken = await TokensService.GetUserWithTokenAsync(Email.Trim(), Password);
            }
            catch (HttpRequestException)
            {
                ResultMessage = Application.Current.Resources["no_server_connection"].ToString();
                return;
            }

            if (userWithToken != null)
            {
                Configuration.User = userWithToken.User;
                Configuration.Token = userWithToken.Token;

                Settings.GetInstance().PreviousUserEmail = userWithToken.User.Email;
                Settings.GetInstance().PreviousUserPassword = userWithToken.User.Password;
                Settings.Store();

                new MainWindow().Show();
                Application.Current.Windows[0].Close();
            }
            else
            {
                ResultMessage = Application.Current.Resources["incorrect_login_details"].ToString();
            }
        }

        private async void Register(object obj = null)
        {
            IsProcessing = true;
            ResultMessage = string.Empty;
            IsResultSuccess = false;

            await CheckRegister();

            IsProcessing = false;
        }

        private async Task CheckRegister()
        {
            if (String.IsNullOrEmpty(Email))
            {
                ResultMessage = Application.Current.Resources["no_email_address_provided"].ToString();
                return;
            }

            if (!new EmailAddressAttribute().IsValid(Email.Trim()))
            {
                ResultMessage = Application.Current.Resources["invalid_email_format"].ToString();
                return;
            }

            if (String.IsNullOrEmpty(Username))
            {
                ResultMessage = Application.Current.Resources["no_username_provided"].ToString();
                return;
            }

            if (String.IsNullOrEmpty(Password))
            {
                ResultMessage = Application.Current.Resources["no_password_provided"].ToString();
                return;
            }

            if (Password != RepeatPassword)
            {
                ResultMessage = Application.Current.Resources["passwords_do_not_match"].ToString();
                return;
            }

            RegisterResult registerResult;

            try
            {
                registerResult = await UsersService.CreateUserAsync(Email.Trim(), Username.Trim(), Password.Trim());

            }
            catch (HttpRequestException)
            {
                ResultMessage = Application.Current.Resources["no_server_connection"].ToString();
                return;
            }

            switch(registerResult)
            {
                case RegisterResult.OK:
                    IsResultSuccess = true;
                    ResultMessage = Application.Current.Resources["account_created_successfully"].ToString();
                    GoToLogIn();
                    break;

                case RegisterResult.OCCUPIED_EMAIL:
                    ResultMessage = Application.Current.Resources["the_given_email_address_is_taken"].ToString();
                    break;

                case RegisterResult.OCCUPIED_USERNAME:
                    ResultMessage = Application.Current.Resources["the_given_username_is_taken"].ToString();
                    break;
            }
        }

        private void GoToLogIn(object obj = null)
        {
            LogInVisibility = Visibility.Visible;
            RegisterVisibility = Visibility.Collapsed;

            if (!_isResultSuccess)
            {
                ResultMessage = string.Empty;
            }
        }

        private void GoToRegister(object obj)
        {
            LogInVisibility = Visibility.Collapsed;
            RegisterVisibility = Visibility.Visible;
            Username = string.Empty;
            ResultMessage = string.Empty;
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

        private void WindowKeyDown(object obj)
        {
            var args = obj as KeyEventArgs;
            if(args != null)
            {
                if(args.Key == Key.Enter && !IsProcessing)
                {
                    if(_registerVisibility == Visibility.Collapsed)
                    {
                        LogIn();
                    }
                    else
                    {
                        Register();
                    }
                }
            }
        }
    }
}
