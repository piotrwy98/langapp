using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface ITranslationsRepository
    {
        Task<IEnumerable<Translation>> GetTranslationsAsync();
        Task<Translation> GetTranslationAsync(uint id);
        Task<Translation> GetTranslationByWordAndLanguageAsync(uint wordId, uint languageId);
        Task<Translation> CreateTranslationAsync(Translation translation);
        Task UpdateTranslationAsync(Translation translation);
        Task DeleteTranslationAsync(uint id);
    }
}
