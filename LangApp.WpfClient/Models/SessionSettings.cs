using LangApp.WpfClient.Converters;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LangApp.WpfClient.Models
{
    public class SessionSettings : NotifyPropertyChanged
    {
        public uint LanguageId { get; set; }
        public uint NumberOfQuestions { get; set; }

        private bool _isClosedChosen;
        public bool IsClosedChosen
        {
            get
            {
                return _isClosedChosen;
            }
            set
            {
                _isClosedChosen = value;
                OnPropertyChanged();
            }
        }

        private bool _isOpenChosen;
        public bool IsOpenChosen
        {
            get
            {
                return _isOpenChosen;
            }
            set
            {
                _isOpenChosen = value;
                OnPropertyChanged();
            }
        }

        private bool _isPronunciationChosen;
        public bool IsPronunciationChosen
        {
            get
            {
                return _isPronunciationChosen;
            }
            set
            {
                _isPronunciationChosen = value;
                OnPropertyChanged();
            }
        }

        public List<uint> CategoriesIds { get; set; }

        public string LanguageInfo
        {
            get
            {
                var languageName = LanguagesService.GetInstance().LanguageNames.First(x => x.LanguageId == LanguageId &&
                    x.SourceLanguageId == Settings.GetInstance().InterfaceLanguageId).Value;

                return languageName;
            }
        }

        public string CategoriesInfo
        {
            get
            {
                string info = string.Empty;

                foreach (var id in CategoriesIds)
                {
                    var category = CategoriesService.GetInstance().Categories.First(x => x.CategoryId == id 
                    && x.LanguageId == Settings.GetInstance().InterfaceLanguageId);

                    info += category.Value + " (" + new LevelNameConverter().Convert(category.Category.Level, null, null, null) + "), ";
                }

                return info.Substring(0, info.Length - 2);
            }
        }

        public SessionSettings()
        {
            CategoriesIds = new List<uint>();
        }
    }
}
