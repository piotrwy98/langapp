using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System;

namespace LangApp.WpfClient.Services
{
    public class FavouriteWordsService : HttpClientService
    {
        private static FavouriteWordsService _instace;

        public ObservableCollection<FavouriteWord> FavouriteWords { get; }

        private FavouriteWordsService()
        {
            FavouriteWords = new ObservableCollection<FavouriteWord>(GetFavouriteWordsOfUserAsync().Result);
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

        public static async Task<FavouriteWord> CreateFavouriteWordAsync(uint firstTranslationId, uint secondTranslationId)
        {
            var favouriteWord = new FavouriteWord()
            {
                UserId = Configuration.GetInstance().User.Id,
                FirstTranslationId = firstTranslationId,
                SecondTranslationId = secondTranslationId
            };

            var content = new StringContent(JsonConvert.SerializeObject(favouriteWord), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("http://localhost:5000/favourite-words", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var newFavouriteWord = JsonConvert.DeserializeObject<FavouriteWord>(json);

                _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _instace.FavouriteWords.Add(newFavouriteWord);
                }));

                foreach (var dictionary in TranslationsService.GetInstance().Dictionaries)
                {
                    foreach (var pair in dictionary.Dictionary)
                    {
                        if ((pair.Value.FirstLanguageTranslation.Id == firstTranslationId &&
                            pair.Value.SecondLanguageTranslation.Id == secondTranslationId) ||
                            (pair.Value.FirstLanguageTranslation.Id == secondTranslationId &&
                            pair.Value.SecondLanguageTranslation.Id == firstTranslationId))
                        {
                            pair.Value.FavouriteWordId = newFavouriteWord.Id;
                        }
                    }
                }

                return newFavouriteWord;
            }

            return null;
        }

        public static async Task<bool> RemoveFavouriteWordAsync(uint id)
        {
            var response = await HttpClient.DeleteAsync("http://localhost:5000/favourite-words/" + id).ConfigureAwait(false);

            if(response.IsSuccessStatusCode)
            {
                var favouriteWord = _instace.FavouriteWords.FirstOrDefault(x => x.Id == id);
                
                _ = Application.Current.Dispatcher.BeginInvoke(new Action(() => 
                {
                    _instace.FavouriteWords.Remove(favouriteWord);
                }));

                foreach(var dictionary in TranslationsService.GetInstance().Dictionaries)
                {
                    foreach(var pair in dictionary.Dictionary)
                    {
                        if(pair.Value.FavouriteWordId == id)
                        {
                            pair.Value.FavouriteWordId = null;
                        }
                    }
                }

                return true;
            }

            return false;
        }
    }
}
