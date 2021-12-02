using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LangApp.WpfClient.Converters
{
    public class CorrectAnswersColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double percent;
            if(value is string valueString)
            {
                percent = double.Parse(valueString.Replace("%", "").Replace(".", ","));
            }
            else
            {
                percent = (double) value;
            }

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
