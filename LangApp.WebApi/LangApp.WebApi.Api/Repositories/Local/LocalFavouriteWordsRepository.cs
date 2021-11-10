using LangApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Local
{
    public class LocalFavouriteWordsRepository : IFavouriteWordsRepository
    {
        private readonly List<FavouriteWord> _favouriteWords = new List<FavouriteWord>()
        {
            new FavouriteWord
            {
                Id = 1, 
                User = LocalUsersRepository.Users[0],
                Word = LocalTranslationsRepository.Words[0], 
                FirstLanguage = LocalTranslationsRepository.Languages[0], 
                SecondLanguage = LocalTranslationsRepository.Languages[1] 
            },
            new FavouriteWord
            {
                Id = 2,
                User = LocalUsersRepository.Users[0],
                Word = LocalTranslationsRepository.Words[1],
                FirstLanguage = LocalTranslationsRepository.Languages[0],
                SecondLanguage = LocalTranslationsRepository.Languages[1]
            },
        };

        public async Task<IEnumerable<FavouriteWord>> GetFavouriteWordsAsync()
        {
            return await Task.FromResult(_favouriteWords);
        }

        public async Task<IEnumerable<FavouriteWord>> GetFavouriteWordsOfUserAsync(uint userId)
        {
            return await Task.FromResult(_favouriteWords.FindAll(x => x.User.Id == userId));
        }

        public async Task<FavouriteWord> GetFavouriteWordAsync(uint id)
        {
            return await Task.FromResult(_favouriteWords.FirstOrDefault(x => x.Id == id));
        }

        public async Task CreateFavouriteWordAsync(FavouriteWord favouriteWord)
        {
            _favouriteWords.Add(favouriteWord);
            await Task.CompletedTask;
        }

        public async Task DeleteFavouriteWordAsync(uint id)
        {
            var index = _favouriteWords.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                _favouriteWords.RemoveAt(index);
            }

            await Task.CompletedTask;
        }
    }
}
