using System;
using System.Globalization;
using System.Windows.Data;

namespace LangApp.WpfClient.Converters
{
    public class DurationMsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds((uint)value);
            return (uint)timeSpan.TotalSeconds + "." + timeSpan.Milliseconds + " s";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
