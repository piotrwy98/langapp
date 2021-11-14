using System;

namespace LangApp.Shared.Models
{
    public partial class FavouriteWord
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public User User { get; set; }
        public uint FirstTranslationId { get; set; }
        public Translation FirstTranslation { get; set; }
        public uint SecondTranslationId { get; set; }
        public Translation SecondTranslation { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
