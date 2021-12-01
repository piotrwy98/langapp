using LangApp.Shared.Models;
using LangApp.WpfClient.Converters;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Controls;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class LearnDetailsViewModel
    {
        #region Commands
        public ICommand ReturnCommand { get; set; }
        #endregion

        #region Properties
        public Session Session { get; }
        public List<Answer> Answers { get; }
        public ChartValues<double> CorrectCount { get; }
        public ChartValues<double> IncorrectCount { get; }
        public ChartValues<double> UnansweredCount { get; }
        public Func<ChartPoint, string> PointLabel { get; set; }
        public string SelectedCategories { get; }
        #endregion

        #region Variables
        private bool _cameFromStats;
        #endregion

        public LearnDetailsViewModel(Session session, List<Answer> answers)
        {
            ReturnCommand = new RelayCommand(Return);

            Session = session;
            Answers = answers;

            if(answers == null)
            {
                _cameFromStats = true;

                Answers = AnswersService.GetInstance().Answers.FindAll(x => x.SessionId == session.Id);
            }

            var correctCount = Answers.Count(x => x.IsAnswerCorrect);
            CorrectCount = new ChartValues<double>();
            CorrectCount.Add(correctCount);

            var incorrectCount = Answers.Count - correctCount;
            IncorrectCount = new ChartValues<double>();
            IncorrectCount.Add(incorrectCount);

            var unansweredCount = Session.QuestionsNumber - correctCount - incorrectCount;
            UnansweredCount = new ChartValues<double>();
            UnansweredCount.Add(unansweredCount);

            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            foreach(var selectedCategory in SelectedCategoriesService.GetInstance().SelectedCategories)
            {
                if(selectedCategory.SessionId == Session.Id)
                {
                    var category = CategoriesService.GetInstance().Categories.First(x => x.CategoryId == selectedCategory.CategoryId && x.LanguageId == Settings.GetInstance().InterfaceLanguageId);
                    SelectedCategories += category.Value + " (" + new LevelNameConverter().Convert(category.Category.Level, null, null, null) + "), ";
                }
            }

            SelectedCategories = SelectedCategories.Substring(0, SelectedCategories.Length - 2);
        }

        private void Return(object obj)
        {
            if(_cameFromStats)
            {
                Configuration.GetInstance().CurrentView = Configuration.GetInstance().StatsControl;
            }
            else
            {
                Configuration.GetInstance().CurrentView = Configuration.GetInstance().LearnFinishControl;
            }
        }
    }
}
