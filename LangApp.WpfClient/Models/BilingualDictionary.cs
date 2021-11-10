using LangApp.Shared.Models;
using System.Collections.Generic;

namespace LangApp.WpfClient.Models
{
    public class BilingualDictionary
    {
        public Language FirstLanguage { get; set; }

        public Language SecondLanguage { get; set; }

        public IDictionary<Word, TranslationSet> Dictionary { get; set; }

        public override string ToString()
        {
            return FirstLanguage.Name + " ➔ " + SecondLanguage.Name;
        }
    }
}
