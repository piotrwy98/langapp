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
        public async static Task<RegisterResult> CreateUserAsync(string email, string username, string password, UserRole role)
        {
            var user = new User()
            {
                Email = email,
                Username = username,
                Password = password,
                Role = role
            };

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("http://localhost:5000/users", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RegisterResult.OK;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RegisterResult>(json);
        }
    }
}
