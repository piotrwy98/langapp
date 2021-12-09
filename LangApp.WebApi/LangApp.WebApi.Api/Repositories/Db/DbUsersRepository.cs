using LangApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Db
{
    public class DbUsersRepository : IUsersRepository
    {
        private readonly LangAppContext _context;

        public DbUsersRepository(LangAppContext langAppContext)
        {
            _context = langAppContext;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await Task.FromResult(_context.Users);
        }

        public async Task<User> GetUserByIdAsync(uint id)
        {
            return await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Where(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var entity = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return await GetUserByIdAsync(entity.Entity.Id);
        }

        public async Task UpdateUserAsync(User user)
        {
            var entity = await _context.Users.FindAsync(user.Id);

            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(uint id)
        {
            var entity = await _context.Users.FindAsync(id);

            if (entity != null)
            {
                _context.Users.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
