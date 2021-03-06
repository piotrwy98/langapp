using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LangApp.WpfClient.Services
{
    public class NewsService : HttpClientService
    {
        private static NewsService _instace;

        public ObservableCollection<Post> Posts { get; }

        private NewsService()
        {
            var news = GetNewsAsync().Result.ToList();
            news = news.OrderByDescending(x => x.CreationDateTime).ToList();
            Posts = new ObservableCollection<Post>();

            foreach(var element in news)
            {
                Posts.Add(new Post(element));
            }
        }

        public static NewsService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new NewsService();
            }

            return _instace;
        }

        private async Task<IEnumerable<News>> GetNewsAsync()
        {
            var response = await HttpClient.GetAsync("news").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<News>>(json);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await GetNewsAsync();
            }

            return null;
        }

        public static async Task<News> CreateNewsAsync(News news)
        {
            var content = new StringContent(JsonConvert.SerializeObject(news), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("news", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<News>(json);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await CreateNewsAsync(news);
            }

            return null;
        }

        public static async Task<bool> UpdateNewsAsync(News news)
        {
            var content = new StringContent(JsonConvert.SerializeObject(news), Encoding.UTF8, "application/json");

            try
            {
                var response = await HttpClient.PutAsync("news", content).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Configuration.RefreshToken();
                    return await UpdateNewsAsync(news);
                }
            }
            catch
            {
                Configuration.GetInstance().NoConnection = true;
            }

            return false;
        }

        public static async Task<bool> RemoveNewsAsync(uint id)
        {
            var response = await HttpClient.DeleteAsync("news/" + id).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await RemoveNewsAsync(id);
            }

            return false;
        }
    }
}
