namespace LangApp.Shared.Models
{
    public class Enums
    {
        public enum UserRole
        {
            USER,
            ADMIN
        }

        public enum RegisterResult
        {
            OK,
            OCCUPIED_EMAIL,
            OCCUPIED_USERNAME
        }

        public enum QuestionType
        {
            CLOSED,
            OPEN,
            PRONUNCIATION
        }

        public enum SessionType
        {
            LEARN,
            TEST
        }

        public enum Level
        {
            A,
            B,
            C
        }
    }
}
