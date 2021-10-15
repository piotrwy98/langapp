namespace LangApp.Shared.Models
{
    public class Enums
    {
        public enum UserRole
        {
            USER = 0,
            ADMIN = 1
        }

        public enum RegisterResult
        {
            OK,
            OCCUPIED_EMAIL,
            OCCUPIED_USERNAME
        }
    }
}
