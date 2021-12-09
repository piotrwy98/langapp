using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LiveCharts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LangApp.WpfClient.Services
{
    public class LanguagesService : HttpClientService
    {
        private static LanguagesService _instace;

        #region Properties
        public List<LanguageName> LanguageNames { get; }

        public List<Language> Languages { get; }
        #endregion

        private LanguagesService()
        {
            LanguageNames = (List<LanguageName>) GetLanguagesAsync().Result;
            Languages = new List<Language>();

            foreach(var languageName in LanguageNames)
            {
                if(!Languages.Contains(languageName.Language))
                {
                    Languages.Add(languageName.Language);
                }
            }
        }

        public static LanguagesService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new LanguagesService();
            }

            return _instace;
        }

        private async Task<IEnumerable<LanguageName>> GetLanguagesAsync()
        {
            var response = await HttpClient.GetAsync("http://localhost:5000/languages").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<LanguageName>>(json);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await GetLanguagesAsync();
            }

            return null;
        }

        public static List<ChartValues<ChartItem>> GetLanguageValues()
        {
            var list = new List<ChartValues<ChartItem>>();

            // dla wszystkich języków
            list.Add(new ChartValues<ChartItem>());

            // dla konkretnego języka
            foreach(var language in GetInstance().Languages)
            {
                list.Add(new ChartValues<ChartItem>());
            }

            return list;
        }
    }
}
