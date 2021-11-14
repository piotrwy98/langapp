using LangApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Db
{
    public class DbLanguagesRepository : ILanguagesRepository
    {
        private readonly LangAppContext _context;

        public DbLanguagesRepository(LangAppContext langAppContext)
        {
            _context = langAppContext;
        }

        public async Task<IEnumerable<LanguageName>> GetLanguagesAsync()
        {
            return await Task.FromResult(_context.LanguageNames.Include(x => x.Language));
        }

        public async Task<LanguageName> GetLanguageAsync(uint id)
        {
            return await _context.LanguageNames.Where(x => x.Id == id)
                .Include(x => x.Language).FirstOrDefaultAsync();
        }

        public async Task<LanguageName> CreateLanguageAsync(LanguageName language)
        {
            var entity = _context.LanguageNames.Add(language);
            await _context.SaveChangesAsync();

            return await GetLanguageAsync(entity.Entity.Id);
        }

        public async Task UpdateLanguageAsync(LanguageName language)
        {
            var entity = await _context.LanguageNames.FindAsync(language.Id);

            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(language);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteLanguageAsync(uint id)
        {
            var entity = await _context.LanguageNames.FindAsync(id);

            if (entity != null)
            {
                _context.LanguageNames.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
