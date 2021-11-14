using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LangApp.WpfClient.Converters
{
    public class LanguageNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return LanguagesService.GetInstance().LanguageNames.FirstOrDefault(
                x => x.SourceLanguageId == Settings.GetInstance().InterfaceLanguageId &&
                x.LanguageId == (uint)value)?.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
