using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    [ApiController]
    [Route("parts-of-speech")]
    public class PartsOfSpeechController : ControllerBase
    {
        private readonly IPartsOfSpeechRepository _partsOfSpeechRepository;

        public PartsOfSpeechController(IPartsOfSpeechRepository partsOfSpeechRepository)
        {
            _partsOfSpeechRepository = partsOfSpeechRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<PartOfSpeechName>> GetPartsOfSpeechAsync()
        {
            return await _partsOfSpeechRepository.GetPartsOfSpeechAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PartOfSpeechName>> GetPartOfSpeechAsync(uint id)
        {
            var partOfSpeech = await _partsOfSpeechRepository.GetPartOfSpeechAsync(id);
            if (partOfSpeech == null)
            {
                return NotFound();
            }

            return partOfSpeech;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<PartOfSpeechName>> CreatePartOfSpeechAsync([FromBody] PartOfSpeechName partOfSpeech)
        {
            return await _partsOfSpeechRepository.CreatePartOfSpeechAsync(partOfSpeech);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdatePartOfSpeechAsync([FromBody] PartOfSpeechName partOfSpeech)
        {
            await _partsOfSpeechRepository.UpdatePartOfSpeechAsync(partOfSpeech);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeletePartOfSpeechAsync(uint id)
        {
            await _partsOfSpeechRepository.DeletePartOfSpeechAsync(id);

            return NoContent();
        }
    }
}
