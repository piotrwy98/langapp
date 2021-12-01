using LangApp.Shared.Models;
using LangApp.WpfClient.ViewModels.Controls;
using System.Collections.Generic;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for LearnDetailsControls.xaml
    /// </summary>
    public partial class LearnDetailsControl : UserControl
    {
        public LearnDetailsControl(Session session, List<Answer> answers)
        {
            InitializeComponent();
            DataContext = new LearnDetailsViewModel(session, answers);
        }
    }
}
