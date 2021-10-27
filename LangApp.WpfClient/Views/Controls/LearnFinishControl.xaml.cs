using LangApp.WpfClient.ViewModels.Controls;
using System.Windows.Controls;

namespace LangApp.WpfClient.Views.Controls
{
    /// <summary>
    /// Interaction logic for LearnFinishControl.xaml
    /// </summary>
    public partial class LearnFinishControl : UserControl
    {
        public LearnFinishControl(string timer, int questionCounter, int numberOfQuestions)
        {
            InitializeComponent();
            DataContext = new LearnFinishViewModel(timer, questionCounter, numberOfQuestions);
        }
    }
}
