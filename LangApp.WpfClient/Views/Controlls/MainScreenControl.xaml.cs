using LangApp.WpfClient.ViewModels.Controlls;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controlls
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
    }
}
