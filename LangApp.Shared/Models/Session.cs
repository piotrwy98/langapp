using System;
using static LangApp.Shared.Models.Enums;

namespace LangApp.Shared.Models
{
    public partial class Session
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public User User { get; set; }
        public uint FirstLanguageId { get; set; }
        public Language FirstLanguage { get; set; }
        public uint SecondLanguageId { get; set; }
        public Language SecondLanguage { get; set; }
        public SessionType Type { get; set; }
        public DateTime StartDateTime { get; set; }
        public uint QuestionsNumber { get; set; }
    }
}
