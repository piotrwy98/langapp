using System;
using System.Globalization;
using System.Windows.Data;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Converters
{
    public class SessionTypeCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(parameter.ToString()) == (int) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool) value == true)
            {
                return int.Parse(parameter.ToString()) == (int) SessionType.LEARN ? SessionType.LEARN : SessionType.TEST;
            }
            else
            {
                return int.Parse(parameter.ToString()) == (int) SessionType.LEARN ? SessionType.TEST : SessionType.LEARN;
            }
        }
    }
}
