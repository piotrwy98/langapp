using System;

namespace LangApp.Shared.Models
{
    public class FavouriteWord
    {
        public uint Id { get; set; }

        public User User { get; set; }

        public Word Word { get; set; }

        public Language FirstLanguage { get; set; }

        public Language SecondLanguage { get; set; }
    }
}
