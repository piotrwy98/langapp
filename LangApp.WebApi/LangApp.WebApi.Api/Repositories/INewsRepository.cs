using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface INewsRepository
    {
        Task<IEnumerable<News>> GetNewsAsync();
        Task<News> GetNewsAsync(uint id);
        Task<News> CreateNewsAsync(News news);
        Task UpdateNewsAsync(News news);
        Task DeleteNewsAsync(uint id);
    }
}
