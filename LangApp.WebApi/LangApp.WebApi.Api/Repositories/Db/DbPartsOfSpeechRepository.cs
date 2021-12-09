using LangApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Db
{
    public class DbPartsOfSpeechRepository : IPartsOfSpeechRepository
    {
        private readonly LangAppContext _context;

        public DbPartsOfSpeechRepository(LangAppContext langAppContext)
        {
            _context = langAppContext;
        }

        public async Task<IEnumerable<PartOfSpeechName>> GetPartsOfSpeechAsync()
        {
            return await Task.FromResult(_context.PartOfSpeechNames.Include(x => x.PartOfSpeech));
        }

        public async Task<PartOfSpeechName> GetPartOfSpeechAsync(uint id)
        {
            return await _context.PartOfSpeechNames.Where(x => x.Id == id)
                .Include(x => x.PartOfSpeech).FirstOrDefaultAsync();
        }

        public async Task<PartOfSpeechName> CreatePartOfSpeechAsync(PartOfSpeechName partOfSpeech)
        {
            var entity = await _context.PartOfSpeechNames.AddAsync(partOfSpeech);
            await _context.SaveChangesAsync();

            return await GetPartOfSpeechAsync(entity.Entity.Id);
        }

        public async Task UpdatePartOfSpeechAsync(PartOfSpeechName partOfSpeech)
        {
            var entity = await _context.PartOfSpeechNames.FindAsync(partOfSpeech.Id);

            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(partOfSpeech);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePartOfSpeechAsync(uint id)
        {
            var entity = await _context.PartOfSpeechNames.FindAsync(id);

            if (entity != null)
            {
                _context.PartOfSpeechNames.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
