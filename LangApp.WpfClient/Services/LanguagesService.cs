using LangApp.Shared.Models;
using System;
using System.Collections.Generic;

namespace LangApp.WpfClient.Services
{
    public class LanguagesService : HttpClientService
    {
        private static LanguagesService _instace;

        public List<Language> Languages;

        private LanguagesService()
        {
            Languages = new List<Language>();
            Languages.Add(new Language { Id = new Guid("00000000000000000000000000000000"), Code = "pl", Name = "Polski", FlagUri = "../../Resources/Flags/pl.png" });
            Languages.Add(new Language { Id = new Guid("00000000000000000000000000000001"), Code = "en", Name = "Angielski", FlagUri = "../../Resources/Flags/en.png" });
        }

        public static LanguagesService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new LanguagesService();
            }

            return _instace;
        }
    }
}
