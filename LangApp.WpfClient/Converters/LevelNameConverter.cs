using System;
using System.Globalization;
using System.Windows.Data;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Converters
{
    public class LevelNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Level)value)
            {
                case Level.A:
                    return "A1/A2";

                case Level.B:
                    return "B1/B2";

                case Level.C:
                    return "C1/C2";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
