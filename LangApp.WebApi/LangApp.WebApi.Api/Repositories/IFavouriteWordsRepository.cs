using LangApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface IFavouriteWordsRepository
    {
        Task<IEnumerable<FavouriteWord>> GetFavouriteWordsAsync();
        Task<IEnumerable<FavouriteWord>> GetFavouriteWordsOfUserAsync(uint userId);
        Task<FavouriteWord> GetFavouriteWordAsync(uint id);
        Task CreateFavouriteWordAsync(FavouriteWord favouriteWord);
        Task DeleteFavouriteWordAsync(uint id);
    }
}
