using LangApp.Shared.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WpfClient.Services
{
    public class CategoriesService : HttpClientService
    {
        private static CategoriesService _instace;

        public List<Category> Categories;

        private CategoriesService()
        {
            Categories = (List<Category>) GetCategoriesAsync().Result;
        }

        public static CategoriesService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new CategoriesService();
            }

            return _instace;
        }

        private async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var response = await HttpClient.GetAsync("http://localhost:5000/categories").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Category>>(json);
            }

            return null;
        }
    }
}
