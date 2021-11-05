using LangApp.WpfClient.Models;
using System.Collections.Generic;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class LearnDetailsViewModel
    {
        #region Commands
        public ICommand ReturnCommand { get; set; }
        #endregion

        #region Properties
        public bool IsTest { get; }

        public List<Answer> Answers { get; }
        #endregion

        public LearnDetailsViewModel(bool isTest, List<Answer> answers)
        {
            IsTest = isTest;
            Answers = answers;

            ReturnCommand = new RelayCommand(Return);
        }

        private void Return(object obj)
        {
            Configuration.GetInstance().CurrentView = Configuration.GetInstance().LearnFinishControl;
        }
    }
}
