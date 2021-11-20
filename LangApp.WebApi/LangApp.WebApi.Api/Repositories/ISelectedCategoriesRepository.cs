using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface ISelectedCategoriesRepository
    {
        Task<IEnumerable<SelectedCategory>> GetSelectedCategoriesAsync();
        Task<SelectedCategory> GetSelectedCategoryAsync(uint id);
        Task<SelectedCategory> CreateSelectedCategoryAsync(SelectedCategory selectedCategory);
        Task UpdateSelectedCategoryAsync(SelectedCategory selectedCategory);
        Task DeleteSelectedCategoryAsync(uint id);
    }
}
