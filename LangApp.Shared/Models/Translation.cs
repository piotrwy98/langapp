namespace LangApp.Shared.Models
{
    public partial class Translation
    {
        public uint Id { get; set; }
        public uint LanguageId { get; set; }
        public Language Language { get; set; }
        public uint WordId { get; set; }
        public Word Word { get; set; }
        public string Value { get; set; }
        public string Phonetic { get; set; }
    }
}
