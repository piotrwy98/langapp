namespace LangApp.WpfClient.ViewModels.Windows
{
    public class ConfirmationViewModel
    {
        public string Title { get; }
        public string Message { get; }

        public ConfirmationViewModel(string title, string message)
        {
            Message = message;
            Title = title;
        }
    }
}
