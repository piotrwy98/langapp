using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface IAnswersRepository
    {
        Task<IEnumerable<Answer>> GetAnswersAsync(uint userId);
        Task<Answer> GetAnswerAsync(uint id);
        Task<Answer> CreateAnswerAsync(Answer answer);
        Task UpdateAnswerAsync(Answer answer);
        Task DeleteAnswerAsync(uint id);
    }
}
