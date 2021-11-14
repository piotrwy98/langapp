using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Local
{
    public class LocalUsersRepository : IUsersRepository
    {
        public static readonly List<User> Users = new List<User>()
        {
            new User { Id = 1, Email = "admin@wp.pl", Password = "admin" }
        };

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await Task.FromResult(Users);
        }

        public async Task<User> GetUserByIdAsync(uint id)
        {
            return await Task.FromResult(Users.FirstOrDefault(x => x.Id == id));
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await Task.FromResult(Users.FirstOrDefault(x => x.Email == email));
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await Task.FromResult(Users.FirstOrDefault(x => x.Username == username));
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.Id = (uint) Users.Count + 1;
            Users.Add(user);

            return await Task.FromResult(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            var index = Users.FindIndex(x => x.Id == user.Id);
            if(index >= 0)
            {
                Users[index] = user;
            }

            await Task.CompletedTask;
        }

        public async Task DeleteUserAsync(uint id)
        {
            var index = Users.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                Users.RemoveAt(index);
            }

            await Task.CompletedTask;
        }
    }
}
