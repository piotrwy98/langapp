using LangApp.WpfClient.ViewModels.Windows;
using System.Windows;
using System.Windows.Input;

namespace LangApp.WpfClient.Views.Windows
{
    /// <summary>
    /// Interaction logic for ConfirmationWindow.xaml
    /// </summary>
    public partial class ConfirmationWindow : Window
    {
        public ConfirmationWindow(string title, string message)
        {
            InitializeComponent();
            DataContext = new ConfirmationViewModel(title, message);
            Owner = Application.Current.Windows[0];
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Yes_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void No_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
