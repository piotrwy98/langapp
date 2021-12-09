using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LangApp.WpfClient.Services
{
    public class PartsOfSpeechService : HttpClientService
    {
        private static PartsOfSpeechService _instace;

        public List<PartOfSpeechName> PartsOfSpeech { get; }

        private PartsOfSpeechService()
        {
            PartsOfSpeech = (List<PartOfSpeechName>) GetPartsOfSpeechAsync().Result;
        }

        public static PartsOfSpeechService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new PartsOfSpeechService();
            }

            return _instace;
        }

        private async Task<IEnumerable<PartOfSpeechName>> GetPartsOfSpeechAsync()
        {
            var response = await HttpClient.GetAsync("http://localhost:5000/parts-of-speech").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<PartOfSpeechName>>(json);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await GetPartsOfSpeechAsync();
            }

            return null;
        }
    }
}
