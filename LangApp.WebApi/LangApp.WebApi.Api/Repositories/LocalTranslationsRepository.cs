using LangApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public class LocalTranslationsRepository : ITranslationsRepository
    {
        private static readonly List<Word> _words = new List<Word>()
        {
            new Word { Id = Guid.NewGuid() },
            new Word { Id = Guid.NewGuid() },
            new Word { Id = Guid.NewGuid() },
            new Word { Id = Guid.NewGuid() },
            new Word { Id = Guid.NewGuid() }
        };

        private static readonly List<Language> _languages = new List<Language>()
        {
            new Language { Id = new Guid("00000000000000000000000000000000"), Code = "pl", Name = "Polski" },
            new Language { Id = new Guid("00000000000000000000000000000001"), Code = "en", Name = "Angielski" }
        };

        private readonly List<Translation> _translations = new List<Translation>()
        {
            new Translation { Id = Guid.NewGuid(), Word = _words[0], Language = _languages[0], Value = "pies" },
            new Translation { Id = Guid.NewGuid(), Word = _words[0], Language = _languages[1], Value = "dog" },
            new Translation { Id = Guid.NewGuid(), Word = _words[1], Language = _languages[0], Value = "kot" },
            new Translation { Id = Guid.NewGuid(), Word = _words[1], Language = _languages[1], Value = "cat" },
            new Translation { Id = Guid.NewGuid(), Word = _words[2], Language = _languages[0], Value = "małpa" },
            new Translation { Id = Guid.NewGuid(), Word = _words[2], Language = _languages[1], Value = "monkey" },
            new Translation { Id = Guid.NewGuid(), Word = _words[3], Language = _languages[0], Value = "słoń" },
            new Translation { Id = Guid.NewGuid(), Word = _words[3], Language = _languages[1], Value = "elephant" },
            new Translation { Id = Guid.NewGuid(), Word = _words[4], Language = _languages[0], Value = "ptak" },
            new Translation { Id = Guid.NewGuid(), Word = _words[4], Language = _languages[1], Value = "bird" },
        };

        public async Task<IEnumerable<Translation>> GetTranslationsAsync(Guid languageId)
        {
            return await Task.FromResult(_translations.FindAll(x => x.Language.Id == languageId));
        }

        public async Task<Translation> GetTranslationAsync(Guid id)
        {
            return await Task.FromResult(_translations.FirstOrDefault(x => x.Id == id));
        }

        public async Task<Translation> GetTranslationByWordAndLanguageAsync(Guid wordId, Guid languageId)
        {
            return await Task.FromResult(_translations.FirstOrDefault(x => x.Word.Id == wordId && x.Language.Id == languageId));
        }

        public async Task CreateTranslationAsync(Translation translation)
        {
            _translations.Add(translation);
            await Task.CompletedTask;
        }

        public async Task UpdateTranslationAsync(Translation translation)
        {
            var index = _translations.FindIndex(x => x.Id == translation.Id);
            if (index >= 0)
            {
                _translations[index] = translation;
            }

            await Task.CompletedTask;
        }

        public async Task DeleteTranslationAsync(Guid id)
        {
            var index = _translations.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                _translations.RemoveAt(index);
            }

            await Task.CompletedTask;
        }
    }
}
