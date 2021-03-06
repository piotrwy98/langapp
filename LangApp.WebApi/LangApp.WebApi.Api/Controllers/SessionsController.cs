using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    [ApiController]
    [Route("sessions")]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionsRepository _sessionsRepository;

        public SessionsController(ISessionsRepository sessionsRepository)
        {
            _sessionsRepository = sessionsRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Session>> GetSessionsAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _sessionsRepository.GetSessionsAsync(uint.Parse(userId));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Session>> GetSessionAsync(uint id)
        {
            var session = await _sessionsRepository.GetSessionAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (session.UserId != uint.Parse(userId))
            {
                return Unauthorized();
            }

            return session;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Session>> CreateSessionAsync([FromBody] Session session)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (session.UserId != uint.Parse(userId))
            {
                return Unauthorized();
            }

            return await _sessionsRepository.CreateSessionAsync(session);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdateSessionAsync([FromBody] Session session)
        {
            await _sessionsRepository.UpdateSessionAsync(session);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeleteSessionAsync(uint id)
        {
            await _sessionsRepository.DeleteSessionAsync(id);

            return NoContent();
        }
    }
}
