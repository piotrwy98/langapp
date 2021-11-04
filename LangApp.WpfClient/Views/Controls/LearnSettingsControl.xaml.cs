using LangApp.WpfClient.ViewModels.Controls;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for LearnSettingsControl.xaml
    /// </summary>
    public partial class LearnSettingsControl : UserControl
    {
        public LearnSettingsControl(bool isTest)
        {
            InitializeComponent();
            DataContext = new LearnSettingsViewModel(isTest);
        }
    }
}
