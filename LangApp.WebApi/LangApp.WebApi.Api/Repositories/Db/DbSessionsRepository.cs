using LangApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Db
{
    public class DbSessionsRepository : ISessionsRepository
    {
        private readonly LangAppContext _context;

        public DbSessionsRepository(LangAppContext langAppContext)
        {
            _context = langAppContext;
        }

        public async Task<IEnumerable<Session>> GetSessionsAsync(uint userId)
        {
            return await Task.FromResult(_context.Sessions.Where(x => x.UserId == userId));
        }

        public async Task<Session> GetSessionAsync(uint id)
        {
            return await _context.Sessions.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Session> CreateSessionAsync(Session session)
        {
            var entity = await _context.Sessions.AddAsync(session);
            await _context.SaveChangesAsync();

            return await GetSessionAsync(entity.Entity.Id);
        }

        public async Task UpdateSessionAsync(Session session)
        {
            var entity = await _context.Sessions.FindAsync(session.Id);

            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteSessionAsync(uint id)
        {
            var entity = await _context.Sessions.FindAsync(id);

            if (entity != null)
            {
                _context.Sessions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
