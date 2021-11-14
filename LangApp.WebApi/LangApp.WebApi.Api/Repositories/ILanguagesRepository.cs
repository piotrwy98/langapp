using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface ILanguagesRepository
    {
        Task<IEnumerable<LanguageName>> GetLanguagesAsync();
        Task<LanguageName> GetLanguageAsync(uint id);
        Task<LanguageName> CreateLanguageAsync(LanguageName language);
        Task UpdateLanguageAsync(LanguageName language);
        Task DeleteLanguageAsync(uint id);
    }
}
