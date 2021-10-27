using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class LearnFinishViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand ChangeCategoryCommand { get; set; }

        public ICommand LearnAgainCommand { get; set; }
        #endregion

        #region Properties
        public string Timer { get; }

        public int QuestionCounter { get; }

        public int NumberOfQuestions { get; }
        #endregion

        public LearnFinishViewModel(string timer, int questionCounter, int numberOfQuestions)
        {
            Timer = timer;
            QuestionCounter = questionCounter;
            NumberOfQuestions = numberOfQuestions;

            ChangeCategoryCommand = new RelayCommand(ChangeCategory);
            LearnAgainCommand = new RelayCommand(LearnAgain);
        }

        private void ChangeCategory(object obj)
        {
            Configuration.GetInstance().CurrentView = Configuration.GetInstance().LearnSettingsControl;
        }

        private void LearnAgain(object obj)
        {
            (Configuration.GetInstance().LearnControl.DataContext as LearnViewModel).Reset();
            Configuration.GetInstance().CurrentView = Configuration.GetInstance().LearnControl;
        }
    }
}
