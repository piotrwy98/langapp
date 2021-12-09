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
    [Route("answers")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswersRepository _answersRepository;
        private readonly ISessionsRepository _sessionsRepository;

        public AnswersController(IAnswersRepository answersRepository, ISessionsRepository sessionsRepository)
        {
            _answersRepository = answersRepository;
            _sessionsRepository = sessionsRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Answer>> GetAnswersAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _answersRepository.GetAnswersAsync(uint.Parse(userId));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Answer>> GetAnswerAsync(uint id)
        {
            var answer = await _answersRepository.GetAnswerAsync(id);

            if (answer != null)
            {
                var session = await _sessionsRepository.GetSessionAsync(answer.SessionId);

                if (session != null)
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (session.UserId != uint.Parse(userId))
                    {
                        return Unauthorized();
                    }

                    return answer;
                }
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Answer>> CreateAnswerAsync([FromBody] Answer answer)
        {
            var session = await _sessionsRepository.GetSessionAsync(answer.SessionId);

            if(session == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (session.UserId != uint.Parse(userId))
            {
                return Unauthorized();
            }

            return await _answersRepository.CreateAnswerAsync(answer);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdateAnswerAsync([FromBody] Answer answer)
        {
            await _answersRepository.UpdateAnswerAsync(answer);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeleteAnswerAsync(uint id)
        {
            await _answersRepository.DeleteAnswerAsync(id);

            return NoContent();
        }
    }
}
