using LangApp.Shared.Models;
using LangApp.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WebApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;

        public UsersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _usersRepository.GetUsersAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserAsync(Guid id)
        {
            var user = await _usersRepository.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUserAsync(string email, string username, string password, UserRole userRole = UserRole.USER)
        {
            User user = new User()
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                Password = password,
                UserRole = userRole
            };

            await _usersRepository.CreateUserAsync(user);

            return CreatedAtAction(nameof(GetUserAsync), new { id = user.Id }, user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUserAsync(User user)
        {
            await _usersRepository.UpdateUserAsync(user);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserAsync(Guid id)
        {
            await _usersRepository.DeleteUserAsync(id);

            return NoContent();
        }
    }
}
