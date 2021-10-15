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
        public async Task<ActionResult<User>> GetUserAsync([FromBody] Guid id)
        {
            var user = await _usersRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> CreateUserAsync([FromBody] RegisterCredentials credentials)
        {
            if(await _usersRepository.GetUserByEmailAsync(credentials.Email) != null)
            {
                return BadRequest(RegisterResult.OCCUPIED_EMAIL);
            }

            if (await _usersRepository.GetUserByUsernameAsync(credentials.Username) != null)
            {
                return BadRequest(RegisterResult.OCCUPIED_USERNAME);
            }

            User user = new User()
            {
                Id = Guid.NewGuid(),
                Email = credentials.Email,
                Username = credentials.Username,
                Password = credentials.Password,
                UserRole = credentials.UserRole
            };

            await _usersRepository.CreateUserAsync(user);

            return CreatedAtAction(nameof(GetUserAsync), new { id = user.Id }, user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUserAsync([FromBody] User user)
        {
            await _usersRepository.UpdateUserAsync(user);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserAsync([FromBody] Guid id)
        {
            await _usersRepository.DeleteUserAsync(id);

            return NoContent();
        }
    }
}
