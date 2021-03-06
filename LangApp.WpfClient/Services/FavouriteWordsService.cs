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
using LangApp.WpfClient.Views.Windows;
using System.Net;

namespace LangApp.WpfClient.Services
{
    public class FavouriteWordsService : HttpClientService
    {
        private static FavouriteWordsService _instace;

        public ObservableCollection<FavouriteWord> FavouriteWords { get; }

        private FavouriteWordsService()
        {
            FavouriteWords = new ObservableCollection<FavouriteWord>(GetFavouriteWordsAsync().Result);
        }

        public static FavouriteWordsService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new FavouriteWordsService();
            }

            return _instace;
        }

        private async Task<IEnumerable<FavouriteWord>> GetFavouriteWordsAsync()
        {
            var response = await HttpClient.GetAsync("favourite-words").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<FavouriteWord>>(json);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await GetFavouriteWordsAsync();
            }

            return null;
        }

        public static async Task CreateFavouriteWordAsync(uint firstTranslationId, uint secondTranslationId)
        {
            var favouriteWord = new FavouriteWord()
            {
                UserId = Configuration.User.Id,
                FirstTranslationId = firstTranslationId,
                SecondTranslationId = secondTranslationId
            };

            var content = new StringContent(JsonConvert.SerializeObject(favouriteWord), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("favourite-words", content).ConfigureAwait(false);

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
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                await CreateFavouriteWordAsync(firstTranslationId, secondTranslationId);
            }
        }

        public static async Task RemoveFavouriteWordAsync(uint id, bool confirmed = false)
        {
            var favouriteWord = _instace.FavouriteWords.First(x => x.Id == id);

            if (!confirmed)
            {
                string favouriteWordInfo = " " + favouriteWord.FirstTranslation.Value + " (" + favouriteWord.SecondTranslation.Value + ") ";

                var confirmationWindow = new ConfirmationWindow(Application.Current.Resources["remove_from_favourites"].ToString(),
                    Application.Current.Resources["remove_from_favourites_confirmation_part1"].ToString() + favouriteWordInfo +
                    Application.Current.Resources["remove_from_favourites_confirmation_part2"].ToString());
                confirmationWindow.ShowDialog();

                if (confirmationWindow.DialogResult != true)
                {
                    return;
                }
            }

            var response = await HttpClient.DeleteAsync("favourite-words/" + id).ConfigureAwait(false);

            if(response.IsSuccessStatusCode)
            {
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
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                await RemoveFavouriteWordAsync(id, true);
            }
        }

        public static void NullInstance()
        {
            _instace = null;
        }
    }
}
