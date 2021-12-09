using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Services
{
    public class UsersService : HttpClientService
    {
        private static UsersService _instace;

        public List<User> Users { get; }

        private UsersService()
        {
            Users = (List<User>) GetUsersAsync().Result;
        }

        public static UsersService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new UsersService();
            }

            return _instace;
        }

        private async Task<IEnumerable<User>> GetUsersAsync()
        {
            var response = await HttpClient.GetAsync("http://localhost:5000/users").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<User>>(json);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await GetUsersAsync();
            }

            return null;
        }

        public async static Task<RegisterResult> CreateUserAsync(string email, string username, string password)
        {
            var user = new User()
            {
                Email = email,
                Username = username,
                Password = password
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
