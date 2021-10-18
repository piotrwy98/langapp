using LangApp.Shared.Models;
using System.Collections.Generic;

namespace LangApp.WpfClient.Models
{
    public class TranslationsList
    {
        public Language Language { get; set; }

        public List<Translation> Translations { get; set; }
    }
}
