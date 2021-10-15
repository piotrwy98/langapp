﻿using LangApp.Shared.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LangApp.WpfClient.Services
{
    public abstract class TokensService : HttpClientService
    {
        public async static Task<UserWithToken> GetUserWithToken(string email, string password)
        {
            var credentials = new LogInCredentials()
            {
                Email = email,
                Password = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await HttpClient.PostAsync("https://localhost:44356/tokens", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserWithToken>(json);
            }

            return null;
        }
    }
}