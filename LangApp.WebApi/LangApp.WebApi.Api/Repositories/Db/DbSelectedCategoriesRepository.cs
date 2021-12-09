using LangApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Db
{
    public class DbSelectedCategoriesRepository : ISelectedCategoriesRepository
    {
        private readonly LangAppContext _context;

        public DbSelectedCategoriesRepository(LangAppContext langAppContext)
        {
            _context = langAppContext;
        }

        public async Task<IEnumerable<SelectedCategory>> GetSelectedCategoriesAsync(uint userId)
        {
            return await Task.FromResult(_context.SelectedCategories.Where(x => x.Session.UserId == userId));
        }

        public async Task<SelectedCategory> GetSelectedCategoryAsync(uint id)
        {
            return await _context.SelectedCategories.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<SelectedCategory> CreateSelectedCategoryAsync(SelectedCategory selectedCategory)
        {
            var entity = await _context.SelectedCategories.AddAsync(selectedCategory);
            await _context.SaveChangesAsync();

            return await GetSelectedCategoryAsync(entity.Entity.Id);
        }

        public async Task UpdateSelectedCategoryAsync(SelectedCategory selectedCategory)
        {
            var entity = await _context.SelectedCategories.FindAsync(selectedCategory.Id);

            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(selectedCategory);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteSelectedCategoryAsync(uint id)
        {
            var entity = await _context.SelectedCategories.FindAsync(id);

            if (entity != null)
            {
                _context.SelectedCategories.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
