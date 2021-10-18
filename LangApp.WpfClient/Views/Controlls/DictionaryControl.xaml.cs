using LangApp.WpfClient.ViewModels.Controlls;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controlls
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
