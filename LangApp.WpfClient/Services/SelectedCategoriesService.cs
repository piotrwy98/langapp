using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LangApp.WpfClient.Services
{
    public class SelectedCategoriesService : HttpClientService
    {
        private static SelectedCategoriesService _instace;

        public List<SelectedCategory> SelectedCategories { get; }

        private SelectedCategoriesService()
        {
            SelectedCategories = (List<SelectedCategory>) GetSelectedCategoriesAsync().Result;
        }

        public static SelectedCategoriesService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new SelectedCategoriesService();
            }

            return _instace;
        }

        private async Task<IEnumerable<SelectedCategory>> GetSelectedCategoriesAsync()
        {
            var response = await HttpClient.GetAsync("selected-categories").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<SelectedCategory>>(json);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await GetSelectedCategoriesAsync();
            }

            return null;
        }

        public static async Task<SelectedCategory> CreateSelectedCategoryAsync(uint sessionId, uint categoryId)
        {
            var selectedCategory = new SelectedCategory()
            {
                SessionId = sessionId,
                CategoryId = categoryId
            };

            var selectedCategories = GetInstance().SelectedCategories;
            var content = new StringContent(JsonConvert.SerializeObject(selectedCategory), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("selected-categories", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                selectedCategory = JsonConvert.DeserializeObject<SelectedCategory>(json);
                selectedCategories.Add(selectedCategory);
                return selectedCategory;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await CreateSelectedCategoryAsync(sessionId, categoryId);
            }

            return null;
        }

        public static void NullInstance()
        {
            _instace = null;
        }
    }
}
