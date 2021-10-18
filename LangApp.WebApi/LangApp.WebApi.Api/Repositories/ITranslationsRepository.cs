using LangApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface ITranslationsRepository
    {
        Task<IEnumerable<Translation>> GetTranslationsAsync(Guid languageId);
        Task<Translation> GetTranslationAsync(Guid id);
        Task<Translation> GetTranslationByWordAndLanguageAsync(Guid wordId, Guid languageId);
        Task CreateTranslationAsync(Translation translation);
        Task UpdateTranslationAsync(Translation translation);
        Task DeleteTranslationAsync(Guid id);
    }
}
