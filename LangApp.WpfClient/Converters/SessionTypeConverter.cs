using System;
using System.Globalization;
using System.Windows.Data;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Converters
{
    public class SessionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((SessionType) value == SessionType.LEARN)
            {
                return "Nauka";
            }

            return "Test";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
