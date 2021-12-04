using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
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
        [Authorize]
        public async Task<IEnumerable<News>> GetNewsAsync()
        {
            return await _newsRepository.GetNewsAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
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
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<News>> CreateNewsAsync([FromBody] News news)
        {
            return await _newsRepository.CreateNewsAsync(news);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdateNewsAsync([FromBody] News news)
        {
            await _newsRepository.UpdateNewsAsync(news);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeleteNewsAsync(uint id)
        {
            await _newsRepository.DeleteNewsAsync(id);

            return NoContent();
        }
    }
}
