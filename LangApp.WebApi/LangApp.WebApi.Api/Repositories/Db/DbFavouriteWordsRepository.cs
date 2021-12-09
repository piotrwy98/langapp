using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories.Db;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Local
{
    public class DbFavouriteWordsRepository : IFavouriteWordsRepository
    {
        private readonly LangAppContext _context;

        public DbFavouriteWordsRepository(LangAppContext langAppContext)
        {
            _context = langAppContext;
        }

        public async Task<IEnumerable<FavouriteWord>> GetFavouriteWordsAsync(uint userId)
        {
            return await Task.FromResult(_context.FavouriteWords.Where(x => x.UserId == userId)
                .Include(x => x.FirstTranslation).ThenInclude(y => y.Language)
                .Include(x => x.SecondTranslation).ThenInclude(y => y.Language));
        }

        public async Task<FavouriteWord> GetFavouriteWordAsync(uint id)
        {
            return await _context.FavouriteWords.Where(x => x.Id == id)
                .Include(x => x.FirstTranslation).ThenInclude(y => y.Language)
                .Include(x => x.SecondTranslation).ThenInclude(y => y.Language)
                .FirstOrDefaultAsync();
        }

        public async Task<FavouriteWord> CreateFavouriteWordAsync(FavouriteWord favouriteWord)
        {
            var entity = await _context.FavouriteWords.AddAsync(favouriteWord);
            await _context.SaveChangesAsync();

            return await GetFavouriteWordAsync(entity.Entity.Id);
        }

        public async Task DeleteFavouriteWordAsync(uint id)
        {
            var entity = await _context.FavouriteWords.FindAsync(id);

            if (entity != null)
            {
                _context.FavouriteWords.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
