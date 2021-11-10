using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("translations")]
    public class TranslationsController : ControllerBase
    {
        private readonly ITranslationsRepository _translationsRepository;

        public TranslationsController(ITranslationsRepository translationsRepository)
        {
            _translationsRepository = translationsRepository;
        }

        [HttpGet("languageId={languageId}")]
        public async Task<IEnumerable<Translation>> GetTranslationsAsync(uint languageId)
        {
            return await _translationsRepository.GetTranslationsAsync(languageId);
        }

        [HttpGet("{id}")]
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
        public async Task<ActionResult<Translation>> CreateTranslationAsync([FromBody] Translation translation)
        {
            if (await _translationsRepository.GetTranslationByWordAndLanguageAsync(translation.Word.Id, translation.Language.Id) != null)
            {
                return BadRequest();
            }

            await _translationsRepository.CreateTranslationAsync(translation);

            return CreatedAtAction(nameof(GetTranslationAsync), new { id = translation.Id }, translation);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateTranslationAsync([FromBody] Translation translation)
        {
            await _translationsRepository.UpdateTranslationAsync(translation);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTranslationAsync(uint id)
        {
            await _translationsRepository.DeleteTranslationAsync(id);

            return NoContent();
        }
    }
}
