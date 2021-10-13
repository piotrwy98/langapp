using LangApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Repositories
{
    public interface IUsersRepository
    {
        Task<bool> DoesUserExistAsync(string email, string password);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserAsync(Guid id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
    }
}
