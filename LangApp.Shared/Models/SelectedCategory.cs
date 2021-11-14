namespace LangApp.Shared.Models
{
    public partial class SelectedCategory
    {
        public uint Id { get; set; }
        public uint SessionId { get; set; }
        public Session Session { get; set; }
        public uint CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
