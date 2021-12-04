using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WebApi.Api.Controllers
{
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
        [Authorize]
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await _usersRepository.GetUsersAsync();

            foreach(var user in users)
            {
                user.Password = null;
            }

            return users;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserAsync(uint id)
        {
            var user = await _usersRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Password = null;

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUserAsync([FromBody] User user)
        {
            if(await _usersRepository.GetUserByEmailAsync(user.Email) != null)
            {
                return BadRequest(RegisterResult.OCCUPIED_EMAIL);
            }

            if (await _usersRepository.GetUserByUsernameAsync(user.Username) != null)
            {
                return BadRequest(RegisterResult.OCCUPIED_USERNAME);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Role = UserRole.USER;

            return await _usersRepository.CreateUserAsync(user);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdateUserAsync([FromBody] User user)
        {
            await _usersRepository.UpdateUserAsync(user);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeleteUserAsync(uint id)
        {
            await _usersRepository.DeleteUserAsync(id);

            return NoContent();
        }
    }
}
