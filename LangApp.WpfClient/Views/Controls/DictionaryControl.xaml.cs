using LangApp.WpfClient.ViewModels.Controls;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for DictionaryControl.xaml
    /// </summary>
    public partial class DictionaryControl : UserControl
    {
        public DictionaryControl()
        {
            InitializeComponent();
            DataContext = new DictionaryViewModel();
        }
    }
}
