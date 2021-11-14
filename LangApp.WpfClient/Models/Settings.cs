namespace LangApp.WpfClient.Models
{
    public class Settings
    {
        private static Settings _instance;

        private Settings() { }

        public static Settings GetInstance()
        {
            if (_instance == null)
                _instance = new Settings();

            return _instance;
        }

        public uint InterfaceLanguageId { get; set; } = 1;
    }
}
