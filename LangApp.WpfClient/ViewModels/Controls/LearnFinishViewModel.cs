using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Views.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class LearnFinishViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand ShowDetailsCommand { get; }
        public ICommand ChangeCategoryCommand { get; }
        public ICommand LearnAgainCommand { get; }
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

        #region Variables
        private Session _session;
        #endregion

        public LearnFinishViewModel(Session session, TimeSpan timer, List<Answer> answers)
        {
            _session = session;
            IsTest = session.Type == SessionType.TEST;
            Timer = timer;
            NumberOfQuestions = session.QuestionsNumber;
            Answers = answers;

            ShowDetailsCommand = new RelayCommand(ShowDetails);
            ChangeCategoryCommand = new RelayCommand(ChangeCategory);
            LearnAgainCommand = new RelayCommand(LearnAgain);

            if(!IsTest)
            {
                ResultText = Application.Current.Resources["learning_completed"].ToString();
                ResultColorBrush = new SolidColorBrush(Color.FromRgb(0x03, 0xA9, 0xF4));
                ResultIcon = PackIconKind.LearnOutline;
            }
            else
            {
                ResultPercent = (double) answers.Count(x => x.IsAnswerCorrect) / NumberOfQuestions * 100;

                if(ResultPercent >= 80)
                {
                    ResultText = Application.Current.Resources["great_job_exc"].ToString();
                    ResultColorBrush = new SolidColorBrush(Colors.LawnGreen);
                    ResultIcon = PackIconKind.FaceExcitedOutline;
                }
                else if (ResultPercent >= 50)
                {
                    ResultText = Application.Current.Resources["pretty_good_exc"].ToString();
                    ResultColorBrush = new SolidColorBrush(Colors.Orange);
                    ResultIcon = PackIconKind.FaceWinkOutline;
                }
                else
                {
                    ResultText = Application.Current.Resources["you_can_do_better_exc"].ToString();
                    ResultColorBrush = new SolidColorBrush(Colors.DarkRed);
                    ResultIcon = PackIconKind.FaceSadOutline;
                }
            }

            if(Configuration.GetInstance().CurrentSchedule != null)
            {
                Configuration.ArrangeSchedule(Configuration.GetInstance().CurrentSchedule);
                Configuration.GetInstance().CurrentSchedule = null;
            }
        }

        private void ShowDetails(object obj)
        {
            Configuration.GetInstance().CurrentView = new LearnDetailsControl(_session, Answers);
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
