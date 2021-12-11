using LangApp.Shared.Models.Controllers;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Windows;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LangApp.WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void ApplicationStartup(object sender, StartupEventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

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

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception.GetType() == typeof(TaskCanceledException) || e.Exception.InnerException != null && 
               (e.Exception.InnerException.GetType() == typeof(HttpRequestException) || e.Exception.InnerException.GetType() == typeof(WebException)))
            {
                Configuration.GetInstance().NoConnection = true;
                e.Handled = true;
            }
        }
    }
}
