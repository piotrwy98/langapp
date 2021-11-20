using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    //[Authorize]
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
        public async Task<IEnumerable<Session>> GetSessionsAsync()
        {
            return await _sessionsRepository.GetSessionsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Session>> GetSessionAsync(uint id)
        {
            var session = await _sessionsRepository.GetSessionAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            return session;
        }

        [HttpPost]
        public async Task<ActionResult<Session>> CreateSessionAsync([FromBody] Session session)
        {
            return await _sessionsRepository.CreateSessionAsync(session);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateSessionAsync([FromBody] Session session)
        {
            await _sessionsRepository.UpdateSessionAsync(session);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSessionAsync(uint id)
        {
            await _sessionsRepository.DeleteSessionAsync(id);

            return NoContent();
        }
    }
}
