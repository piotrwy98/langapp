namespace LangApp.WpfClient.Models
{
    public static class Extensions
    {
        public static string GetName(this QuestionType questionType)
        {
            switch(questionType)
            {
                case QuestionType.CLOSED:
                    return "Zamknięte";

                case QuestionType.OPEN:
                    return "Otwarte";

                case QuestionType.PRONUNCIATION:
                    return "Wymowa";
            }

            return null;
        }
    }
}
