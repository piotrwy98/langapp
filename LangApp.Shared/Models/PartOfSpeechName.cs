namespace LangApp.Shared.Models
{
    public partial class PartOfSpeechName
    {
        public uint Id { get; set; }
        public uint PartOfSpeechId { get; set; }
        public PartOfSpeech PartOfSpeech { get; set; }
        public uint LanguageId { get; set; }
        public Language Language { get; set; }
        public string Value { get; set; }
    }
}
