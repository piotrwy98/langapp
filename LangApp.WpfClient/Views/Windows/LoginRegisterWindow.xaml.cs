using LangApp.WpfClient.ViewModels.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LangApp.WpfClient.Views.Windows
{
    /// <summary>
    /// Interaction logic for LoginRegisterWindow.xaml
    /// </summary>
    public partial class LoginRegisterWindow : Window
    {
        public LoginRegisterWindow(bool serverFailed)
        {
            InitializeComponent();
            DataContext = new LoginRegisterViewModel(serverFailed);
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

        private void MinimizeWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RepeatPassword_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                Password_PasswordBox.Password = "";
                RepeatPassword_PasswordBox.Password = "";
            }
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Focus();
            (sender as TextBox).SelectAll();
        }
    }
}
