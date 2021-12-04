using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LangApp.WpfClient.Services
{
    public class NewsService : HttpClientService
    {
        private static NewsService _instace;

        public ObservableCollection<ObjectToChoose> News { get; }

        private NewsService()
        {
            var news = GetNewsAsync().Result.ToList();
            news = news.OrderByDescending(x => x.CreationDateTime).ToList();
            News = new ObservableCollection<ObjectToChoose>();

            foreach(var post in news)
            {
                News.Add(new ObjectToChoose()
                {
                    Object = post,
                });
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
            var response = await HttpClient.GetAsync("http://localhost:5000/news").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<News>>(json);
            }

            return null;
        }

        public static async Task<News> CreateNewsAsync(News news)
        {
            var content = new StringContent(JsonConvert.SerializeObject(news), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("http://localhost:5000/news", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<News>(json);
            }

            return null;
        }

        public static async Task<bool> UpdateNewsAsync(News news)
        {
            var content = new StringContent(JsonConvert.SerializeObject(news), Encoding.UTF8, "application/json");
            var response = await HttpClient.PutAsync("http://localhost:5000/news", content).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> RemoveNewsAsync(uint id)
        {
            var response = await HttpClient.DeleteAsync("http://localhost:5000/news/" + id).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }
    }
}
