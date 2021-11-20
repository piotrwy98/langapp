using static LangApp.Shared.Models.Enums;

namespace LangApp.Shared.Models
{
    public partial class Answer
    {
        public uint Id { get; set; }
        public uint SessionId { get; set; }
        public Session Session { get; set; }
        public uint NumberInSession { get; set; }
        public QuestionType QuestionType { get; set; }
        public string Value { get; set; }
        public string CorrectAnswer { get; set; }
        public uint DurationMs { get; set; }
        public bool IsAnswerCorrect
        {
            get
            {
                return Value == CorrectAnswer;
            }
        }
    }
}
