using LangApp.WpfClient.Models;
using LangApp.WpfClient.ViewModels.Controls;
using System.Collections.Generic;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for LearnDetailsControls.xaml
    /// </summary>
    public partial class LearnDetailsControls : UserControl
    {
        public LearnDetailsControls(bool isTest, List<Answer> answers)
        {
            InitializeComponent();
            DataContext = new LearnDetailsViewModel(isTest, answers);
        }
    }
}
