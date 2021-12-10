using LangApp.WpfClient.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LangApp.WpfClient.Services
{
    public abstract class HttpClientService
    {
        private static readonly HttpClient _httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };
        protected static HttpClient HttpClient
        {
            get
            {
                Configuration.GetInstance().NoConnection = false;
                return _httpClient;
            }
        }

        public static void SetToken(string token)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
