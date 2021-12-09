using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories.Db;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Local
{
    public class DbCategoriesRepository : ICategoriesRepository
    {
        private readonly LangAppContext _context;

        public DbCategoriesRepository(LangAppContext langAppContext)
        {
            _context = langAppContext;
        }

        public async Task<IEnumerable<CategoryName>> GetCategoriesAsync()
        {
            return await Task.FromResult(_context.CategoryNames.Include(x => x.Category));
        }

        public async Task<CategoryName> GetCategoryAsync(uint id)
        {
            return await _context.CategoryNames.Where(x => x.Id == id)
                .Include(x => x.Category).FirstOrDefaultAsync();
        }

        public async Task<CategoryName> CreateCategoryAsync(CategoryName category)
        {
            var entity = await _context.CategoryNames.AddAsync(category);
            await _context.SaveChangesAsync();

            return await GetCategoryAsync(entity.Entity.Id);
        }

        public async Task UpdateCategoryAsync(CategoryName category)
        {
            var entity = await _context.CategoryNames.FindAsync(category.Id);

            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCategoryAsync(uint id)
        {
            var entity = await _context.CategoryNames.FindAsync(id);

            if (entity != null)
            {
                _context.CategoryNames.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
