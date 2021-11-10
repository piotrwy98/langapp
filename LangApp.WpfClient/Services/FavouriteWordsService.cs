using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.ViewModels.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WpfClient.Services
{
    public class FavouriteWordsService : HttpClientService
    {
        private static FavouriteWordsService _instace;

        #region Properties
        public List<FavouriteWord> FavouriteWords { get; }
        #endregion

        private FavouriteWordsService()
        {
            FavouriteWords = (List<FavouriteWord>) GetFavouriteWordsOfUserAsync().Result;
        }

        public static FavouriteWordsService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new FavouriteWordsService();
            }

            return _instace;
        }

        private async Task<IEnumerable<FavouriteWord>> GetFavouriteWordsOfUserAsync()
        {
            var response = await HttpClient.GetAsync("http://localhost:5000/favourite-words/user/" + Configuration.GetInstance().User.Id).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<FavouriteWord>>(json);
            }

            return null;
        }

        public static async Task<FavouriteWord> CreateFavouriteWordAsync(User user, Word word, Language firstLanguage, Language secondLanguage)
        {
            var favouriteWord = new FavouriteWord()
            {
                User = user,
                Word = word,
                FirstLanguage = firstLanguage,
                SecondLanguage = secondLanguage
            };

            var content = new StringContent(JsonConvert.SerializeObject(favouriteWord), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("http://localhost:5000/favourite-words", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FavouriteWord>(json);
            }

            return null;
        }

        public static async Task<bool> RemoveFavouriteWordAsync(uint id)
        {
            var response = await HttpClient.DeleteAsync("http://localhost:5000/favourite-words/" + id).ConfigureAwait(false);

            if(response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}
