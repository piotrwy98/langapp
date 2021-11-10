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
            Languages.Add(new Language { Id = 0, Code = "pl", Name = "Polski", ImagePath = "../../../Resources/Flags/pl.png" });
            Languages.Add(new Language { Id = 1, Code = "en", Name = "Angielski", ImagePath = "../../../Resources/Flags/en.png" });
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
