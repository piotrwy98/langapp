using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Local
{
    public class LocalTranslationsRepository : ITranslationsRepository
    {
        public static readonly List<Word> Words = new List<Word>()
        {
            new Word { Id = 1, ImagePath = "https://cdn.pixabay.com/photo/2015/11/24/04/01/golden-retriever-1059490_960_720.jpg" },
            new Word { Id = 2, ImagePath = "https://cdn.pixabay.com/photo/2015/11/23/10/39/cat-1058095_960_720.jpg" },
            new Word { Id = 3, ImagePath = "https://cdn.pixabay.com/photo/2018/09/25/21/32/monkey-3703230_960_720.jpg" },
            new Word { Id = 4, ImagePath = "https://cdn.pixabay.com/photo/2013/05/29/22/25/elephant-114543_960_720.jpg" },
            new Word { Id = 5, ImagePath = "https://cdn.pixabay.com/photo/2021/06/12/17/51/great-spotted-woodpecker-6331448_960_720.jpg" }
        };

        public static readonly List<Language> Languages = new List<Language>()
        {
            new Language { Id = 1, Code = "pl", ImagePath = "../../../Resources/Flags/pl.png" },
            new Language { Id = 2, Code = "en", ImagePath = "../../../Resources/Flags/en.png" }
        };

        public static readonly List<Translation> Translations = new List<Translation>()
        {
            new Translation { Id = 1, Word = Words[0], Language = Languages[0], Value = "pies" },
            new Translation { Id = 2, Word = Words[0], Language = Languages[1], Value = "dog" },
            new Translation { Id = 3, Word = Words[1], Language = Languages[0], Value = "kot" },
            new Translation { Id = 4, Word = Words[1], Language = Languages[1], Value = "cat" },
            new Translation { Id = 5, Word = Words[2], Language = Languages[0], Value = "małpa" },
            new Translation { Id = 6, Word = Words[2], Language = Languages[1], Value = "monkey" },
            new Translation { Id = 7, Word = Words[3], Language = Languages[0], Value = "słoń" },
            new Translation { Id = 8, Word = Words[3], Language = Languages[1], Value = "elephant" },
            new Translation { Id = 9, Word = Words[4], Language = Languages[0], Value = "ptak" },
            new Translation { Id = 10, Word = Words[4], Language = Languages[1], Value = "bird" },
        };

        public async Task<IEnumerable<Translation>> GetTranslationsAsync()
        {
            return await Task.FromResult(Translations);
        }

        public async Task<Translation> GetTranslationAsync(uint id)
        {
            return await Task.FromResult(Translations.FirstOrDefault(x => x.Id == id));
        }

        public async Task<Translation> GetTranslationByWordAndLanguageAsync(uint wordId, uint languageId)
        {
            return await Task.FromResult(Translations.FirstOrDefault(x => x.Word.Id == wordId && x.Language.Id == languageId));
        }

        public async Task<Translation> CreateTranslationAsync(Translation translation)
        {
            translation.Id = (uint) Translations.Count + 1;
            Translations.Add(translation);

            return await Task.FromResult(translation);
        }

        public async Task UpdateTranslationAsync(Translation translation)
        {
            var index = Translations.FindIndex(x => x.Id == translation.Id);
            if (index >= 0)
            {
                Translations[index] = translation;
            }

            await Task.CompletedTask;
        }

        public async Task DeleteTranslationAsync(uint id)
        {
            var index = Translations.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                Translations.RemoveAt(index);
            }

            await Task.CompletedTask;
        }
    }
}
