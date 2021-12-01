using LangApp.Shared.Models;
using LangApp.WpfClient.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LangApp.WpfClient.Converters
{
    public class CorrectAnswersPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var session = value as Session;
            var correctAnswers = AnswersService.GetInstance().Answers.Count(x => x.SessionId == session.Id && x.IsAnswerCorrect);

            return 100.0 * correctAnswers / session.QuestionsNumber;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
