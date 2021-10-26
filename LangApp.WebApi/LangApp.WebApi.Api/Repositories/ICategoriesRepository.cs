using LangApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface ICategoriesRepository
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryAsync(Guid id);
        Task CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Guid id);
    }
}
