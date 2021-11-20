using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Services
{
    public class SessionsService : HttpClientService
    {
        private static SessionsService _instace;

        public List<Session> Sessions { get; }

        private SessionsService()
        {
            Sessions = (List<Session>) GetSessionsAsync().Result;
        }

        public static SessionsService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new SessionsService();
            }

            return _instace;
        }

        private async Task<IEnumerable<Session>> GetSessionsAsync()
        {
            var response = await HttpClient.GetAsync("http://localhost:5000/sessions").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Session>>(json);
            }

            return null;
        }

        public static async Task<Session> CreateSessionAsync(uint firstLangugeId, uint secondLangugeId, SessionType type)
        {
            var session = new Session()
            {
                UserId = Configuration.GetInstance().User.Id,
                FirstLanguageId = firstLangugeId,
                SecondLanguageId = secondLangugeId,
                Type = type
            };

            var content = new StringContent(JsonConvert.SerializeObject(session), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("http://localhost:5000/sessions", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                session = JsonConvert.DeserializeObject<Session>(json);
                GetInstance().Sessions.Add(session);
                return session;
            }

            return null;
        }
    }
}
