using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    //[Authorize]
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
        public async Task<IEnumerable<FavouriteWord>> GetFavouriteWordsAsync()
        {
            return await _favouriteWordsRepository.GetFavouriteWordsAsync();
        }

        [HttpGet("user/{id}")]
        public async Task<IEnumerable<FavouriteWord>> GetFavouriteWordsOfUserAsync(uint userId)
        {
            return await _favouriteWordsRepository.GetFavouriteWordsOfUserAsync(userId);
        }

        [HttpGet("{id}")]
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
        public async Task<ActionResult<FavouriteWord>> CreateFavouriteWordAsync([FromBody] FavouriteWord favouriteWord)
        {
            await _favouriteWordsRepository.CreateFavouriteWordAsync(favouriteWord);

            return CreatedAtAction(nameof(GetFavouriteWordAsync), new { id = favouriteWord.Id }, favouriteWord);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFavouriteWordAsync(uint id)
        {
            await _favouriteWordsRepository.DeleteFavouriteWordAsync(id);

            return NoContent();
        }
    }
}
