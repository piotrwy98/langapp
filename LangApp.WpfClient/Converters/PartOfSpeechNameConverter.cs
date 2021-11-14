using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LangApp.WpfClient.Converters
{
    public class PartOfSpeechNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return PartsOfSpeechService.GetInstance().PartsOfSpeech.FirstOrDefault(
                x => x.LanguageId == Settings.GetInstance().InterfaceLanguageId &&
                x.PartOfSpeechId == (uint)value)?.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
