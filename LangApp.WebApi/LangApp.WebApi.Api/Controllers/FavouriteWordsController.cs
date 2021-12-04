using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            return await _favouriteWordsRepository.GetFavouriteWordsAsync();
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IEnumerable<FavouriteWord>> GetFavouriteWordsOfUserAsync(uint userId)
        {
            return await _favouriteWordsRepository.GetFavouriteWordsOfUserAsync(userId);
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

            return favouriteWord;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<FavouriteWord>> CreateFavouriteWordAsync([FromBody] FavouriteWord favouriteWord)
        {
            return await _favouriteWordsRepository.CreateFavouriteWordAsync(favouriteWord);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteFavouriteWordAsync(uint id)
        {
            await _favouriteWordsRepository.DeleteFavouriteWordAsync(id);

            return NoContent();
        }
    }
}
