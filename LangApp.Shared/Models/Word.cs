namespace LangApp.Shared.Models
{
    public partial class Word
    {
        public uint Id { get; set; }
        public uint CategoryId { get; set; }
        public Category Category { get; set; }
        public uint PartOfSpeechId { get; set; }
        public PartOfSpeech PartOfSpeech { get; set; }
        public string ImagePath { get; set; }
    }
}
