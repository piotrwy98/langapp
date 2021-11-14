using static LangApp.Shared.Models.Enums;

namespace LangApp.Shared.Models
{
    public partial class Category
    {
        public uint Id { get; set; }
        public Level Level { get; set; }
        public string ImagePath { get; set; }
    }
}
