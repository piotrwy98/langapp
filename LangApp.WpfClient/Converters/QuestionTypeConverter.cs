using System;
using System.Globalization;
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
                    return "Zamknięte";

                case QuestionType.OPEN:
                    return "Otwarte";

                case QuestionType.PRONUNCIATION:
                    return "Wymowa";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
