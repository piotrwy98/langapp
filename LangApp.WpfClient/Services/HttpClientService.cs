using System.Net.Http;
using System.Net.Http.Headers;

namespace LangApp.WpfClient.Services
{
    public abstract class HttpClientService
    {
        protected static readonly HttpClient HttpClient = new HttpClient();

        public static void SetToken(string token)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
