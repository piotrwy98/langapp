using LangApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Db
{
    public class DbAnswersRepository : IAnswersRepository
    {
        private readonly LangAppContext _context;

        public DbAnswersRepository(LangAppContext langAppContext)
        {
            _context = langAppContext;
        }

        public async Task<IEnumerable<Answer>> GetAnswersAsync()
        {
            return await Task.FromResult(_context.Answers);
        }

        public async Task<Answer> GetAnswerAsync(uint id)
        {
            return await _context.Answers.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Answer> CreateAnswerAsync(Answer answer)
        {
            var entity = _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return await GetAnswerAsync(entity.Entity.Id);
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            var entity = await _context.Answers.FindAsync(answer.Id);

            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(answer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAnswerAsync(uint id)
        {
            var entity = await _context.Answers.FindAsync(id);

            if (entity != null)
            {
                _context.Answers.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
