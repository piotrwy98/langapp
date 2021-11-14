using System;

namespace LangApp.Shared.Models
{
    public partial class News
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
