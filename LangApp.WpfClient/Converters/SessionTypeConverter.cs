using System;
using System.Globalization;
using System.Windows;
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
                return Application.Current.Resources["learn"].ToString();
            }

            return Application.Current.Resources["test"].ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
