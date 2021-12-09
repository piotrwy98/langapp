using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories.Db;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Local
{
    public class DbTranslationsRepository : ITranslationsRepository
    {
        private readonly LangAppContext _context;

        public DbTranslationsRepository(LangAppContext langAppContext)
        {
            _context = langAppContext;
        }

        public async Task<IEnumerable<Translation>> GetTranslationsAsync()
        {
            return await Task.FromResult(_context.Translations.Include(x => x.Word));
        }

        public async Task<Translation> GetTranslationAsync(uint id)
        {
            return await _context.Translations.Where(x => x.Id == id)
                .Include(x => x.Word).FirstOrDefaultAsync();
        }

        public async Task<Translation> GetTranslationByWordAndLanguageAsync(uint wordId, uint languageId)
        {
            return await _context.Translations.Where(x => x.WordId == wordId && x.LanguageId == languageId)
                .Include(x => x.Word).FirstOrDefaultAsync();
        }

        public async Task<Translation> CreateTranslationAsync(Translation translation)
        {
            var entity = await _context.Translations.AddAsync(translation);
            await _context.SaveChangesAsync();

            return await GetTranslationAsync(entity.Entity.Id);
        }

        public async Task UpdateTranslationAsync(Translation translation)
        {
            var entity = await _context.Translations.FindAsync(translation.Id);

            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(translation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTranslationAsync(uint id)
        {
            var entity = await _context.Translations.FindAsync(id);

            if (entity != null)
            {
                _context.Translations.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
