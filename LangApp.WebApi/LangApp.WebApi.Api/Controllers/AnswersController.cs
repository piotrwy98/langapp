using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    [ApiController]
    [Route("answers")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswersRepository _answersRepository;

        public AnswersController(IAnswersRepository answersRepository)
        {
            _answersRepository = answersRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Answer>> GetAnswersAsync()
        {
            return await _answersRepository.GetAnswersAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Answer>> GetAnswerAsync(uint id)
        {
            var answer = await _answersRepository.GetAnswerAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            return answer;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Answer>> CreateAnswerAsync([FromBody] Answer answer)
        {
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
