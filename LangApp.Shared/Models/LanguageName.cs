namespace LangApp.Shared.Models
{
    public partial class LanguageName
    {
        public uint Id { get; set; }
        public uint SourceLanguageId { get; set; }
        public Language SourceLanguage { get; set; }
        public uint LanguageId { get; set; }
        public Language Language { get; set; }
        public string Value { get; set; }
    }
}
