using LangApp.WpfClient.ViewModels.Controls;
using System.Windows;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for MainScreenControl.xaml
    /// </summary>
    public partial class MainScreenControl : UserControl
    {
        public MainScreenControl()
        {
            InitializeComponent();
            DataContext = new MainScreenViewModel();
        }

        private void Title_TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Focus();
            (sender as TextBox).SelectAll();
        }
    }
}
