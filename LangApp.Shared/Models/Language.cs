using System;

namespace LangApp.Shared.Models
{
    public class Language
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string FlagUri { get; set; }
    }
}
