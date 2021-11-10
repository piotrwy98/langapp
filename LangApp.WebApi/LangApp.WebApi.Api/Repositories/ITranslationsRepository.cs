using LangApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface ITranslationsRepository
    {
        Task<IEnumerable<Translation>> GetTranslationsAsync(uint languageId);
        Task<Translation> GetTranslationAsync(uint id);
        Task<Translation> GetTranslationByWordAndLanguageAsync(uint wordId, uint languageId);
        Task CreateTranslationAsync(Translation translation);
        Task UpdateTranslationAsync(Translation translation);
        Task DeleteTranslationAsync(uint id);
    }
}
