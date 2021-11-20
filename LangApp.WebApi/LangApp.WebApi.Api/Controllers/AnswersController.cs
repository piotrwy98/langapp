using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    //[Authorize]
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
        public async Task<IEnumerable<Answer>> GetAnswersAsync()
        {
            return await _answersRepository.GetAnswersAsync();
        }

        [HttpGet("{id}")]
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
        public async Task<ActionResult<Answer>> CreateAnswerAsync([FromBody] Answer answer)
        {
            return await _answersRepository.CreateAnswerAsync(answer);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAnswerAsync([FromBody] Answer answer)
        {
            await _answersRepository.UpdateAnswerAsync(answer);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAnswerAsync(uint id)
        {
            await _answersRepository.DeleteAnswerAsync(id);

            return NoContent();
        }
    }
}
