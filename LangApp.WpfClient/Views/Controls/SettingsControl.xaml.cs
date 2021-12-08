using LangApp.WpfClient.Models;
using LangApp.WpfClient.ViewModels.Controls;
using System.Windows;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }

        private void Mode_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Store();
        }
    }
}
