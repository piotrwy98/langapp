using LangApp.WpfClient.ViewModels.Controls;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for FavouriteWordsControl.xaml
    /// </summary>
    public partial class FavouriteWordsControl : UserControl
    {
        public FavouriteWordsControl()
        {
            InitializeComponent();
            DataContext = new FavouriteWordsViewModel();
        }
    }
}
