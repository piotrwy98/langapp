using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace LangApp.WpfClient.Models
{
    public class Settings
    {
        private static readonly byte[] AES_KEY = new byte[] { 100, 133, 219, 8, 231, 92, 77, 133, 208, 49, 153, 19, 110, 71, 72, 176, 182, 15, 107, 132, 152, 223, 102, 232, 34, 222, 62, 148, 129, 93, 53, 247 };
        private static readonly byte[] AES_IV = new byte[] { 13, 164, 108, 23, 147, 68, 104, 60, 112, 107, 24, 197, 151, 130, 83, 65 };
        private static readonly string FILE_PATH = "settings.bin";
        
        private static Settings _instance;

        private uint _interfaceLanguageId = 1;
        public uint InterfaceLanguageId
        {
            get
            {
                return _interfaceLanguageId;
            }
            set
            {
                _interfaceLanguageId = value;

                ResourceDictionary resourceDictionary = new ResourceDictionary();

                if (_interfaceLanguageId == 1)
                {
                    resourceDictionary.Source = new Uri("..\\..\\Resources\\Languages\\pl-PL.xaml", UriKind.Relative);
                }
                else
                {
                    resourceDictionary.Source = new Uri("..\\..\\Resources\\Languages\\en-US.xaml", UriKind.Relative);
                }

                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }

        public string PreviousUserEmail { get; set; }
        public string PreviousUserPassword { get; set; }
        public bool StartWithSystem { get; set; }
        public bool MinimizeToSystemTray { get; set; }
        public ObservableCollection<Schedule> Schedules { get; set; }

        private Settings()
        {
            Schedules = new ObservableCollection<Schedule>();
        }

        public static Settings GetInstance()
        {
            if (_instance == null)
            {
                _instance = Restore();
                
                if(_instance == null)
                {
                    _instance = new Settings();
                }
            }

            return _instance;
        }

        public static void Store()
        {
            using (var aesManaged = new AesManaged())
            {
                var encryptor = aesManaged.CreateEncryptor(AES_KEY, AES_IV);

                using (var memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            var json = JsonConvert.SerializeObject(GetInstance());
                            streamWriter.Write(json);
                        }

                        File.WriteAllBytes(FILE_PATH, memoryStream.ToArray());
                    }
                }
            }
        }

        private static Settings Restore()
        {
            try
            {
                using (var aesManaged = new AesManaged())
                {
                    var decryptor = aesManaged.CreateDecryptor(AES_KEY, AES_IV);
                    var bytes = File.ReadAllBytes(FILE_PATH);

                    using (var memoryStream = new MemoryStream(bytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (var streamReader = new StreamReader(cryptoStream))
                            {
                                var json = streamReader.ReadToEnd();
                                return JsonConvert.DeserializeObject<Settings>(json);
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
