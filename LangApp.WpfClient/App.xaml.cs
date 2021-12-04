using LangApp.Shared.Models.Controllers;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Windows;
using System.Windows;

namespace LangApp.WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void ApplicationStartup(object sender, StartupEventArgs e)
        {
            bool serverFailed = false;

            if(Settings.GetInstance().PreviousUserEmail != null)
            {
                UserWithToken userWithToken = null;

                try
                {
                    userWithToken = await TokensService.GetUserWithTokenAsync(Settings.GetInstance().PreviousUserEmail,
                        Settings.GetInstance().PreviousUserPassword);
                }
                catch
                {
                    serverFailed = true;
                }

                if (userWithToken != null)
                {
                    Configuration.User = userWithToken.User;
                    Configuration.Token = userWithToken.Token;

                    new MainWindow().Show();
                    return;
                }
            }

            var loginRegisterWindow = new LoginRegisterWindow(serverFailed);
            loginRegisterWindow.Show();
        }
    }
}
