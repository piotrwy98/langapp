using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Views.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class LearnFinishViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand ShowDetailsCommand { get; set; }
        public ICommand ChangeCategoryCommand { get; set; }
        public ICommand LearnAgainCommand { get; set; }
        #endregion

        #region Properties
        public bool IsTest { get; }

        public TimeSpan Timer { get; }

        public uint NumberOfQuestions { get; }

        public List<Answer> Answers { get; }

        public string ResultText { get; }

        public SolidColorBrush ResultColorBrush { get; }

        public PackIconKind ResultIcon { get; }

        public double ResultPercent { get; }
        #endregion

        public LearnFinishViewModel(bool isTest, TimeSpan timer, uint numberOfQuestions, List<Answer> answers)
        {
            IsTest = isTest;
            Timer = timer;
            NumberOfQuestions = numberOfQuestions;
            Answers = answers;

            ShowDetailsCommand = new RelayCommand(ShowDetails);
            ChangeCategoryCommand = new RelayCommand(ChangeCategory);
            LearnAgainCommand = new RelayCommand(LearnAgain);

            if(!isTest)
            {
                ResultText = "Zakończono naukę";
                ResultColorBrush = new SolidColorBrush(Color.FromRgb(0x03, 0xA9, 0xF4));
                ResultIcon = PackIconKind.LearnOutline;
            }
            else
            {
                ResultPercent = (double) answers.Count(x => x.IsAnswerCorrect) / numberOfQuestions * 100;

                if(ResultPercent >= 80)
                {
                    ResultText = "Świetna robota!";
                    ResultColorBrush = new SolidColorBrush(Colors.LawnGreen);
                    ResultIcon = PackIconKind.FaceExcitedOutline;
                }
                else if (ResultPercent >= 50)
                {
                    ResultText = "Całkiem nieźle!";
                    ResultColorBrush = new SolidColorBrush(Colors.Orange);
                    ResultIcon = PackIconKind.FaceWinkOutline;
                }
                else
                {
                    ResultText = "Potrafisz lepiej!";
                    ResultColorBrush = new SolidColorBrush(Colors.Red);
                    ResultIcon = PackIconKind.FaceSadOutline;
                }
            }
        }

        private void ShowDetails(object obj)
        {
            Configuration.GetInstance().CurrentView = new LearnDetailsControls(IsTest, Answers);
        }

        private void ChangeCategory(object obj)
        {
            if(IsTest)
            {
                Configuration.GetInstance().CurrentView = Configuration.GetInstance().TestSettingsControl;
            }
            else
            {
                Configuration.GetInstance().CurrentView = Configuration.GetInstance().LearnSettingsControl;
            }
        }

        private void LearnAgain(object obj)
        {
            if (IsTest)
            {
                (Configuration.GetInstance().TestSettingsControl.DataContext as LearnSettingsViewModel).StartLearning();
            }
            else
            {
                (Configuration.GetInstance().LearnSettingsControl.DataContext as LearnSettingsViewModel).StartLearning();
            }
        }
    }
}
