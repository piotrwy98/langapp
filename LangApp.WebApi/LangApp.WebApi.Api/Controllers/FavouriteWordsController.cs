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
    [Route("favourite-words")]
    public class FavouriteWordsController : ControllerBase
    {
        private readonly IFavouriteWordsRepository _favouriteWordsRepository;

        public FavouriteWordsController(IFavouriteWordsRepository favouriteWordsRepository)
        {
            _favouriteWordsRepository = favouriteWordsRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<FavouriteWord>> GetFavouriteWordsAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _favouriteWordsRepository.GetFavouriteWordsAsync(uint.Parse(userId));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<FavouriteWord>> GetFavouriteWordAsync(uint id)
        {
            var favouriteWord = await _favouriteWordsRepository.GetFavouriteWordAsync(id);
            
            if (favouriteWord == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (favouriteWord.UserId != uint.Parse(userId))
            {
                return Unauthorized();
            }

            return favouriteWord;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<FavouriteWord>> CreateFavouriteWordAsync([FromBody] FavouriteWord favouriteWord)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (favouriteWord.UserId != uint.Parse(userId))
            {
                return Unauthorized();
            }

            return await _favouriteWordsRepository.CreateFavouriteWordAsync(favouriteWord);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateFavouriteWordAsync([FromBody] FavouriteWord favouriteWord)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (favouriteWord.UserId != uint.Parse(userId))
            {
                return Unauthorized();
            }

            await _favouriteWordsRepository.UpdateFavouriteWordAsync(favouriteWord);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteFavouriteWordAsync(uint id)
        {
            var favouriteWord = await _favouriteWordsRepository.GetFavouriteWordAsync(id);

            if (favouriteWord == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (favouriteWord.UserId != uint.Parse(userId))
            {
                return Unauthorized();
            }

            await _favouriteWordsRepository.DeleteFavouriteWordAsync(id);

            return NoContent();
        }
    }
}
