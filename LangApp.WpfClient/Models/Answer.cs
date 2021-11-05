using System;

namespace LangApp.WpfClient.Models
{
    public class Answer
    {
        public int Index { get; set; }

        public string QuestionType { get; set; }

        public string UserAnswer { get; set; }

        public string CorrectAnswer { get; set; }

        public bool IsAnswerCorrect { get; set; }

        public TimeSpan Duration { get; set; }

        public string AnswerResult
        {
            get
            {
                if (IsAnswerCorrect)
                    return "Poprawna";

                return "Niepoprawna";
            }
        }
    }
}
