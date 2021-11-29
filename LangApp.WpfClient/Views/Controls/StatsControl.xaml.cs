using LangApp.WpfClient.ViewModels.Controls;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for StatsControl.xaml
    /// </summary>
    public partial class StatsControl : UserControl
    {
        public StatsControl()
        {
            InitializeComponent();
            DataContext = new StatsViewModel();
        }
    }
}
