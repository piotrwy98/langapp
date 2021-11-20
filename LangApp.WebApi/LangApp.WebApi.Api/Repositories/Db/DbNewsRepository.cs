using LangApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Db
{
    public class DbNewsRepository : INewsRepository
    {
        private readonly LangAppContext _context;

        public DbNewsRepository(LangAppContext langAppContext)
        {
            _context = langAppContext;
        }

        public async Task<IEnumerable<News>> GetNewsAsync()
        {
            return await Task.FromResult(_context.News);
        }

        public async Task<News> GetNewsAsync(uint id)
        {
            return await _context.News.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<News> CreateNewsAsync(News news)
        {
            var entity = _context.News.Add(news);
            await _context.SaveChangesAsync();

            return await GetNewsAsync(entity.Entity.Id);
        }

        public async Task UpdateNewsAsync(News news)
        {
            var entity = await _context.News.FindAsync(news.Id);

            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(news);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteNewsAsync(uint id)
        {
            var entity = await _context.News.FindAsync(id);

            if (entity != null)
            {
                _context.News.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
