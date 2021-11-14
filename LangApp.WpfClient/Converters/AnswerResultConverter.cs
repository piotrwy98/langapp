using System;
using System.Globalization;
using System.Windows.Data;

namespace LangApp.WpfClient.Converters
{
    public class AnswerResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "Poprawna";
            }

            return "Niepoprawna";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
