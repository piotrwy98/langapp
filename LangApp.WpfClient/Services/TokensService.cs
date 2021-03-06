using LangApp.Shared.Models.Controllers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LangApp.WpfClient.Services
{
    public abstract class TokensService : HttpClientService
    {
        public static async Task<UserWithToken> GetUserWithTokenAsync(string email, string password)
        {
            var logInData = new LogInData()
            {
                Email = email,
                Password = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(logInData), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await HttpClient.PostAsync("tokens", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserWithToken>(json);
            }

            return null;
        }
    }
}
