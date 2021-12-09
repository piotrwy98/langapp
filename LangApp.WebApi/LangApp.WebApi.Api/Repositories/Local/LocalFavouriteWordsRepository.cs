using LangApp.Shared.Models;
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
                UserId = 1,
                FirstTranslationId = 1,
                FirstTranslation = LocalTranslationsRepository.Translations[0],
                SecondTranslationId = 2,
                SecondTranslation = LocalTranslationsRepository.Translations[1] 
            },
            new FavouriteWord
            {
                Id = 2,
                UserId = 1,
                FirstTranslationId = 3,
                FirstTranslation = LocalTranslationsRepository.Translations[2],
                SecondTranslationId = 4,
                SecondTranslation = LocalTranslationsRepository.Translations[3]
            }
        };

        public async Task<IEnumerable<FavouriteWord>> GetFavouriteWordsAsync(uint userId)
        {
            return await Task.FromResult(_favouriteWords.FindAll(x => x.UserId == userId));
        }

        public async Task<FavouriteWord> GetFavouriteWordAsync(uint id)
        {
            return await Task.FromResult(_favouriteWords.FirstOrDefault(x => x.Id == id));
        }

        public async Task<FavouriteWord> CreateFavouriteWordAsync(FavouriteWord favouriteWord)
        {
            favouriteWord.Id = (uint) _favouriteWords.Count + 1;
            _favouriteWords.Add(favouriteWord);

            return await Task.FromResult(favouriteWord);
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
