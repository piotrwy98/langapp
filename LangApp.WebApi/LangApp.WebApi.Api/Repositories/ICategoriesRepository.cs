using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface ICategoriesRepository
    {
        Task<IEnumerable<CategoryName>> GetCategoriesAsync();
        Task<CategoryName> GetCategoryAsync(uint id);
        Task<CategoryName> CreateCategoryAsync(CategoryName category);
        Task UpdateCategoryAsync(CategoryName category);
        Task DeleteCategoryAsync(uint id);
    }
}
