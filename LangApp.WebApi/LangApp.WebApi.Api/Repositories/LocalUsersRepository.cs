using LangApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Repositories
{
    public class LocalUsersRepository : IUsersRepository
    {
        private readonly List<User> _users = new List<User>()
        {
            new User { Id = Guid.NewGuid(), Email = "1", Password = "2" }
        };

        public async Task<bool> DoesUserExistAsync(string email, string password)
        {
            return await Task.FromResult(_users.Any(x => x.Email == email && x.Password == password));
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await Task.FromResult(_users);
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            return await Task.FromResult(_users.FirstOrDefault(x => x.Id == id));
        }

        public async Task CreateUserAsync(User user)
        {
            _users.Add(user);
            await Task.CompletedTask;
        }

        public async Task UpdateUserAsync(User user)
        {
            var index = _users.FindIndex(x => x.Id == user.Id);
            if(index >= 0)
            {
                _users[index] = user;
            }

            await Task.CompletedTask;
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var index = _users.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                _users.RemoveAt(index);
            }

            await Task.CompletedTask;
        }
    }
}
