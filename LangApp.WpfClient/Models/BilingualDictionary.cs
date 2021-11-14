using LangApp.Shared.Models;
using LangApp.WpfClient.Converters;
using System.Collections.Generic;

namespace LangApp.WpfClient.Models
{
    public class BilingualDictionary
    {
        public Language FirstLanguage { get; set; }

        public Language SecondLanguage { get; set; }

        public IDictionary<Word, TranslationSet> Dictionary { get; set; }

        public override string ToString()
        {
            LanguageNameConverter languageNameConverter = new LanguageNameConverter();
            var firstLanguage = languageNameConverter.Convert(FirstLanguage.Id, null, null, null).ToString();
            var secondLanguage = languageNameConverter.Convert(SecondLanguage.Id, null, null, null).ToString();

            return firstLanguage + " ➔ " + secondLanguage;
        }
    }
}
