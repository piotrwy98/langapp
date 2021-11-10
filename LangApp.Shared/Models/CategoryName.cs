namespace LangApp.Shared.Models
{
    public class CategoryName
    {
        public uint Id { get; set; }
        public Category Category { get; set; }
        public Language Language { get; set; }
        public string Name { get; set; }
    }
}
