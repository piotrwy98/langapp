using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("news")]
    public class NewsController : ControllerBase
    {
        private readonly INewsRepository _newsRepository;

        public NewsController(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<News>> GetNewsAsync()
        {
            return await _newsRepository.GetNewsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<News>> GetNewsAsync(uint id)
        {
            var news = await _newsRepository.GetNewsAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            return news;
        }

        [HttpPost]
        public async Task<ActionResult<News>> CreateNewsAsync([FromBody] News news)
        {
            return await _newsRepository.CreateNewsAsync(news);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateNewsAsync([FromBody] News news)
        {
            await _newsRepository.UpdateNewsAsync(news);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNewsAsync(uint id)
        {
            await _newsRepository.DeleteNewsAsync(id);

            return NoContent();
        }
    }
}
