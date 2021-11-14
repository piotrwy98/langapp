using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories
{
    public interface IPartsOfSpeechRepository
    {
        Task<IEnumerable<PartOfSpeechName>> GetPartsOfSpeechAsync();
        Task<PartOfSpeechName> GetPartOfSpeechAsync(uint id);
        Task<PartOfSpeechName> CreatePartOfSpeechAsync(PartOfSpeechName partOfSpeech);
        Task UpdatePartOfSpeechAsync(PartOfSpeechName partOfSpeech);
        Task DeletePartOfSpeechAsync(uint id);
    }
}
