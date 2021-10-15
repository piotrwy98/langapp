using LangApp.Shared.Models;
using LangApp.WpfClient.Services;

namespace LangApp.WpfClient.Models
{
    public class Configuration
    {
        private static Configuration _instance;

        private Configuration() { }

        public static Configuration GetInstance()
        {
            if (_instance == null)
                _instance = new Configuration();

            return _instance;
        }

        private string _token;
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                HttpClientService.SetToken(value);
            }
        }

        public User User { get; set; }
    }
}
