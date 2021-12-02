using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Converters
{
    class QuestionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((QuestionType)value)
            {
                case QuestionType.CLOSED:
                    return Application.Current.Resources["closed"].ToString();

                case QuestionType.OPEN:
                    return Application.Current.Resources["open"].ToString();

                case QuestionType.PRONUNCIATION:
                    return Application.Current.Resources["pronunciation"].ToString();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
