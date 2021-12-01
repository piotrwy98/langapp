using LangApp.WpfClient.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LangApp.WpfClient.Converters
{
    public class LanguageFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return LanguagesService.GetInstance().Languages.FirstOrDefault(
                x => x.Id == (uint)value).ImagePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
