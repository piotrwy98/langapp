using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WebApi.Api.Controllers
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
        public async Task<ActionResult<User>> GetUserAsync(uint id)
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

            return await _usersRepository.CreateUserAsync(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUserAsync([FromBody] User user)
        {
            await _usersRepository.UpdateUserAsync(user);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserAsync(uint id)
        {
            await _usersRepository.DeleteUserAsync(id);

            return NoContent();
        }
    }
}
