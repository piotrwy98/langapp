using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.ViewModels.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for LearnControl.xaml
    /// </summary>
    public partial class LearnControl : UserControl
    {
        public LearnControl(Session session, SessionSettings sessionSettings)
        {
            InitializeComponent();
            DataContext = new LearnViewModel(session, sessionSettings);
        }

        private void TextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as TextBox).IsEnabled)
            {
                (sender as TextBox).Focus();
            }
            else
            {
                Keyboard.ClearFocus();
            }
        }

        private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((sender as TextBox).IsVisible)
            {
                (sender as TextBox).Focus();
            }
        }
    }
}
