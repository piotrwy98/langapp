using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    [ApiController]
    [Route("translations")]
    public class TranslationsController : ControllerBase
    {
        private readonly ITranslationsRepository _translationsRepository;

        public TranslationsController(ITranslationsRepository translationsRepository)
        {
            _translationsRepository = translationsRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Translation>> GetTranslationsAsync()
        {
            return await _translationsRepository.GetTranslationsAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Translation>> GetTranslationAsync(uint id)
        {
            var translation = await _translationsRepository.GetTranslationAsync(id);
            if (translation == null)
            {
                return NotFound();
            }

            return translation;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<Translation>> CreateTranslationAsync([FromBody] Translation translation)
        {
            return await _translationsRepository.CreateTranslationAsync(translation);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdateTranslationAsync([FromBody] Translation translation)
        {
            await _translationsRepository.UpdateTranslationAsync(translation);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeleteTranslationAsync(uint id)
        {
            await _translationsRepository.DeleteTranslationAsync(id);

            return NoContent();
        }
    }
}
