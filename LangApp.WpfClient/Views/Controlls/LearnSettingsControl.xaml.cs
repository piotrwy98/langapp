using LangApp.WpfClient.ViewModels.Controlls;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controlls
{
    /// <summary>
    /// Interaction logic for LearnSettingsControl.xaml
    /// </summary>
    public partial class LearnSettingsControl : UserControl
    {
        public LearnSettingsControl()
        {
            InitializeComponent();
            DataContext = new LearnSettingsViewModel();
        }
    }
}
