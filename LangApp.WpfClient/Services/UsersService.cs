using LangApp.Shared.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Services
{
    public abstract class UsersService : HttpClientService
    {
        public async static Task<RegisterResult> CreateUser(string email, string username, string password, UserRole userRole)
        {
            var credentials = new RegisterCredentials()
            {
                Email = email,
                Username = username,
                Password = password,
                UserRole = userRole
            };

            var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await HttpClient.PostAsync("https://localhost:44356/users", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RegisterResult.OK;
            }
            else
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RegisterResult>(json);
            }
        }
    }
}
