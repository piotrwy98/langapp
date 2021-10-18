using System;

namespace LangApp.Shared.Models
{
    public class Translation
    {
        public Guid Id { get; set; }

        public Language Language { get; set; }

        public Word Word { get; set; }

        public string Value { get; set; }
    }
}
