using LangApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
