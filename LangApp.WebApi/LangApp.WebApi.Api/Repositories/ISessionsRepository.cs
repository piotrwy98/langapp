using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface ISessionsRepository
    {
        Task<IEnumerable<Session>> GetSessionsAsync(uint userId);
        Task<Session> GetSessionAsync(uint id);
        Task<Session> CreateSessionAsync(Session session);
        Task UpdateSessionAsync(Session session);
        Task DeleteSessionAsync(uint id);
    }
}
