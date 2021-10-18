using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LangApp.WpfClient.Services
{
    public class TranslationsService : HttpClientService
    {
        private static TranslationsService _instace;

        public List<TranslationsList> TranslationsLists;

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
    }
}
