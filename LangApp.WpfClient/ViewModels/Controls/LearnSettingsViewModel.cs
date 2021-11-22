using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using LangApp.WpfClient.Views.Controls;
using LangApp.Shared.Models;
using System.Threading.Tasks;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class LearnSettingsViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand LanguageClickCommand { get; set; }

        public ICommand NumberClickCommand { get; set; }

        public ICommand CategoryClickCommand { get; set; }

        public ICommand ClosedClickCommand { get; set; }

        public ICommand OpenClickCommand { get; set; }

        public ICommand SpeakClickCommand { get; set; }

        public ICommand StartLearningCommand { get; set; }
        #endregion

        #region Properties
        public SessionType SessionType { get; }

        public bool IsTest
        {
            get
            {
                return SessionType == SessionType.TEST;
            }
        }

        public List<ObjectToChoose> Languages { get; }

        public List<ObjectToChoose> QuestionNumbers { get; }

        public List<ObjectToChoose> Categories { get; }

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

        private bool _isSpeakChosen;
        public bool IsSpeakChosen
        {
            get
            {
                return _isSpeakChosen;
            }
            set
            {
                _isSpeakChosen = value;
                OnPropertyChanged();
            }
        }

        public bool CanStartLearning
        {
            get
            {
                return (IsClosedChosen || IsOpenChosen || IsSpeakChosen) &&
                    Categories.Count(x => x.IsChosen) > 0;
            }
        }
        #endregion

        public LearnSettingsViewModel(SessionType sessionType)
        {
            SessionType = sessionType;
            LanguageClickCommand = new RelayCommand(LanguageClick);
            NumberClickCommand = new RelayCommand(NumberClick);
            CategoryClickCommand = new RelayCommand(CategoryClick);
            ClosedClickCommand = new RelayCommand(ClosedClick);
            OpenClickCommand = new RelayCommand(OpenClick);
            SpeakClickCommand = new RelayCommand(SpeakClick);
            StartLearningCommand = new RelayCommand(StartLearning);

            uint id = Settings.GetInstance().InterfaceLanguageId;
            var languages = LanguagesService.GetInstance().LanguageNames.FindAll(x => x.SourceLanguageId == id && x.LanguageId != id);
            Languages = new List<ObjectToChoose>();

            foreach (var language in languages)
            {
                Languages.Add(new ObjectToChoose()
                {
                    Object = language
                });
            }

            Languages[0].IsChosen = true;

            QuestionNumbers = new List<ObjectToChoose>();
            QuestionNumbers.Add(new ObjectToChoose() { Object = 5, IsChosen = true });
            QuestionNumbers.Add(new ObjectToChoose() { Object = 10 });
            QuestionNumbers.Add(new ObjectToChoose() { Object = 20 });

            var categories = CategoriesService.GetInstance().Categories.FindAll(x => x.LanguageId == id);
            Categories = new List<ObjectToChoose>();

            foreach(var category in categories)
            {
                Categories.Add(new ObjectToChoose()
                {
                    Object = category
                });
            }
        }

        private void ClosedClick(object obj)
        {
            IsClosedChosen = !IsClosedChosen;
            OnPropertyChanged("CanStartLearning");
        }

        private void OpenClick(object obj)
        {
            IsOpenChosen = !IsOpenChosen;
            OnPropertyChanged("CanStartLearning");
        }

        private void SpeakClick(object obj)
        {
            IsSpeakChosen = !IsSpeakChosen;
            OnPropertyChanged("CanStartLearning");
        }

        private void LanguageClick(object obj)
        {
            var objectToChoose = obj as ObjectToChoose;
            if (objectToChoose != null && !objectToChoose.IsChosen)
            {
                foreach(var language in Languages)
                {
                    language.IsChosen = false;
                }

                objectToChoose.IsChosen = true;
            }
        }

        private void NumberClick(object obj)
        {
            var objectToChoose = obj as ObjectToChoose;
            if (objectToChoose != null && !objectToChoose.IsChosen)
            {
                foreach (var number in QuestionNumbers)
                {
                    number.IsChosen = false;
                }

                objectToChoose.IsChosen = true;
            }
        }

        private void CategoryClick(object obj)
        {
            var objectToChoose = obj as ObjectToChoose;
            if(objectToChoose != null)
            {
                objectToChoose.IsChosen = !objectToChoose.IsChosen;
                OnPropertyChanged("CanStartLearning");
            }
        }

        public async void StartLearning(object obj = null)
        {
            var language = (Languages.First(x => x.IsChosen).Object as LanguageName).Language;
            var numberOfQuestions = (int) QuestionNumbers.First(x => x.IsChosen).Object;

            List<uint> categoriesIds = new List<uint>();
            foreach(var category in Categories)
            {
                if(category.IsChosen)
                {
                    categoriesIds.Add((category.Object as CategoryName).Category.Id);
                }
            }

            // utworzenie sesji
            var session = await Task.Run(() => SessionsService.CreateSessionAsync(Settings.GetInstance().InterfaceLanguageId, language.Id, SessionType));
            
            if(session != null)
            {
                // dodanie wybranych kategori do utworzonej sesji
                foreach(var id in categoriesIds)
                {
                    await Task.Run(() => SelectedCategoriesService.CreateSelectedCategoryAsync(session.Id, id));
                }

                if (SessionType == SessionType.TEST)
                {
                    Configuration.GetInstance().TestControl = new LearnControl(true, session.Id, language, categoriesIds, _isClosedChosen, _isOpenChosen, _isSpeakChosen, numberOfQuestions);
                    Configuration.GetInstance().CurrentView = Configuration.GetInstance().TestControl;
                }
                else
                {
                    Configuration.GetInstance().LearnControl = new LearnControl(false, session.Id, language, categoriesIds, _isClosedChosen, _isOpenChosen, _isSpeakChosen, numberOfQuestions);
                    Configuration.GetInstance().CurrentView = Configuration.GetInstance().LearnControl;
                }
            }
        }
    }
}
