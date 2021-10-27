using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LangApp.WpfClient.Services
{
    public class TranslationsService : HttpClientService
    {
        private static TranslationsService _instace;

        #region Properties
        public List<TranslationsList> TranslationsLists { get; }

        public List<BilingualDictionary> Dictionaries { get; }
        #endregion

        private TranslationsService()
        {
            TranslationsLists = new List<TranslationsList>();

            var languages = LanguagesService.GetInstance().Languages;

            foreach (var language in languages)
            {
                TranslationsLists.Add(new TranslationsList()
                {
                    Language = language,
                    Translations = (List<Translation>)GetTranslationsAsync(language.Id).Result
                });
            }

            Dictionaries = new List<BilingualDictionary>();
            GenerateDictionaries();
        }

        public static TranslationsService GetInstance()
        {
            if(_instace == null)
            {
                _instace = new TranslationsService();
            }

            return _instace;
        }

        private async Task<IEnumerable<Translation>> GetTranslationsAsync(Guid languageId)
        {
            var response = await HttpClient.GetAsync("http://localhost:5000/translations/languageId=" + languageId).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Translation>>(json);
            }

            return null;
        }

        private void GenerateDictionaries()
        {
            for (int firstLangIndex = 0; firstLangIndex < TranslationsLists.Count; firstLangIndex++)
            {
                for (int secondLangIndex = firstLangIndex + 1; secondLangIndex < TranslationsLists.Count; secondLangIndex++)
                {
                    var firstDicitionary = GetDictionary(firstLangIndex, secondLangIndex);

                    Dictionaries.Add(new BilingualDictionary()
                    {
                        FirstLanguage = TranslationsLists[firstLangIndex].Language,
                        SecondLanguage = TranslationsLists[secondLangIndex].Language,
                        Dictionary = firstDicitionary
                    });

                    var secondDictionary = new Dictionary<Word, TranslationSet>();

                    foreach (var pair in firstDicitionary)
                    {
                        secondDictionary.Add(pair.Key, new TranslationSet()
                        {
                            FirstLanguageTranslation = pair.Value.SecondLanguageTranslation,
                            SecondLanguageTranslation = pair.Value.FirstLanguageTranslation
                        });
                    }

                    Dictionaries.Add(new BilingualDictionary()
                    {
                        FirstLanguage = TranslationsLists[secondLangIndex].Language,
                        SecondLanguage = TranslationsLists[firstLangIndex].Language,
                        Dictionary = secondDictionary
                    });
                }
            }
        }

        private Dictionary<Word, TranslationSet> GetDictionary(int firstLangIndex, int secondLangIndex)
        {
            var dictionary = new Dictionary<Word, TranslationSet>();

            foreach (var translation in TranslationsLists[firstLangIndex].Translations)
            {
                Translation secondTranslation = TranslationsLists[secondLangIndex].Translations.FirstOrDefault(x => x.Word.Id == translation.Word.Id);

                if (secondTranslation != null)
                {
                    dictionary.Add(translation.Word, new TranslationSet()
                    {
                        FirstLanguageTranslation = translation.Value,
                        SecondLanguageTranslation = secondTranslation.Value,
                    });
                }
            }

            return dictionary;
        }
    }
}
