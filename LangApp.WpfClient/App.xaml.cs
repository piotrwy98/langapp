using LangApp.Shared.Models.Controllers;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Windows;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
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
        private Mutex _mutex;
        private EventWaitHandle _eventWaitHandle;

        private async void ApplicationStartup(object sender, StartupEventArgs e)
        {
            // może istnieć tylko jedna instancja aplikacji jednocześnie
            CheckOtherInstancesExistance();

            // potrzebne, gdy aplikacja jest uruchamiana przy starcie systemu
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // ustawienia HTTPS
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

        private void CheckOtherInstancesExistance()
        {
            _mutex = new Mutex(true, "LangAppMutex", out bool createdNew);
            _eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "LangAppEvent");
            GC.KeepAlive(_mutex);

            if (createdNew)
            {
                var thread = new Thread(() =>
                {
                    while (_eventWaitHandle.WaitOne())
                    {
                        Current.Dispatcher.BeginInvoke(
                            (Action)(() => ((MainWindow)Current.MainWindow).EnsureVisibility()));
                    }
                });

                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                _eventWaitHandle.Set();
                Shutdown();
            }
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // obsługa wyjątków związanych z błędem lub brakiem połączenia
            if (e.Exception.GetType() == typeof(TaskCanceledException) || e.Exception.InnerException != null && 
               (e.Exception.InnerException.GetType() == typeof(HttpRequestException) || e.Exception.InnerException.GetType() == typeof(WebException)))
            {
                Configuration.GetInstance().NoConnection = true;
                e.Handled = true;
            }
        }
    }
}
