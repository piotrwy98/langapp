using LangApp.WpfClient.Views.Windows;
using System.Windows;

namespace LangApp.WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            //var loginRegisterWindow = new LoginRegisterWindow();
            //loginRegisterWindow.Show();

            new MainWindow().Show();
        }
    }
}
