namespace LangApp.Shared.Models
{
    public partial class CategoryName
    {
        public uint Id { get; set; }
        public uint LanguageId { get; set; }
        public Language Language { get; set; }
        public uint CategoryId { get; set; }
        public Category Category { get; set; }
        public string Value { get; set; }
    }
}
