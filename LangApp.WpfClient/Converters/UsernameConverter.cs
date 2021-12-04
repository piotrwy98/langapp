using LangApp.WpfClient.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LangApp.WpfClient.Converters
{
    public class UsernameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return UsersService.GetInstance().Users.First(x => x.Id == (uint)value).Username;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
