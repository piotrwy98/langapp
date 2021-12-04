using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    [ApiController]
    [Route("languages")]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguagesRepository _languagesRepository;

        public LanguagesController(ILanguagesRepository languagesRepository)
        {
            _languagesRepository = languagesRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<LanguageName>> GetLanguagesAsync()
        {
            return await _languagesRepository.GetLanguagesAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<LanguageName>> GetLanguageAsync(uint id)
        {
            var language = await _languagesRepository.GetLanguageAsync(id);
            if (language == null)
            {
                return NotFound();
            }

            return language;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<LanguageName>> CreateLanguageAsync([FromBody] LanguageName language)
        {
            return await _languagesRepository.CreateLanguageAsync(language);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdateLanguageAsync([FromBody] LanguageName language)
        {
            await _languagesRepository.UpdateLanguageAsync(language);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeleteLanguageAsync(uint id)
        {
            await _languagesRepository.DeleteLanguageAsync(id);

            return NoContent();
        }
    }
}
