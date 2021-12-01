using LangApp.Shared.Models;
using LangApp.WpfClient.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace LangApp.WpfClient.Converters
{
    public class CorrectAnswersColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var session = value as Session;
            var correctAnswers = AnswersService.GetInstance().Answers.Count(x => x.SessionId == session.Id && x.IsAnswerCorrect);
            var percent = 100.0 * correctAnswers / session.QuestionsNumber;

            if (percent >= 80)
            {
                return new SolidColorBrush(Colors.LawnGreen);
            }

            if (percent >= 50)
            {
                return new SolidColorBrush(Colors.Orange);
            }

            return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
