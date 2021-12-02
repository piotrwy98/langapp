using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.ViewModels.Controls;
using LangApp.WpfClient.Views.Controls;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Converters
{
    public class CategoriesPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var categoryId = (value as CategoryName).CategoryId;
            var dataContext = (Configuration.GetInstance().CurrentView as LearnSettingsControl).DataContext as LearnSettingsViewModel;
            var sessionType = dataContext.SessionType;
            var secondLanguageId = (dataContext.Languages[0].Object as LanguageName).LanguageId;
            var firstLanguageId = Settings.GetInstance().InterfaceLanguageId;
            var dictionary = TranslationsService.GetInstance().Dictionaries.First(x => x.FirstLanguage.Id == firstLanguageId && x.SecondLanguage.Id == secondLanguageId);

            var totalCount = dictionary.Dictionary.Count(x => x.Key.CategoryId == categoryId);

            if(totalCount == 0)
            {
                return 0;
            }

            double answeredCount;

            if(sessionType == SessionType.LEARN)
            {
                answeredCount = dictionary.Dictionary.Count(x => x.Key.CategoryId == categoryId && x.Value.LearnCount > 0);
            }
            else
            {
                answeredCount = dictionary.Dictionary.Count(x => x.Key.CategoryId == categoryId && x.Value.TestCount > 0);
            }

            return 100.0 * answeredCount / totalCount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
