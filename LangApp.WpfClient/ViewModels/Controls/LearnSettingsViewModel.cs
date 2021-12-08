using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using LangApp.WpfClient.Views.Controls;
using LangApp.Shared.Models;
using System.Threading.Tasks;
using static LangApp.Shared.Models.Enums;
using System;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class LearnSettingsViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand LanguageClickCommand { get; }

        public ICommand NumberClickCommand { get; }

        public ICommand CategoryClickCommand { get; }

        public ICommand ClosedClickCommand { get; }

        public ICommand OpenClickCommand { get; }

        public ICommand PronunciationClickCommand { get; }

        public ICommand StartLearningCommand { get; }
        #endregion

        #region Properties
        public SessionType? SessionType { get; }

        public bool? IsTest
        {
            get
            {
                if(SessionType == null)
                {
                    return null;
                }

                return SessionType == Enums.SessionType.TEST;
            }
        }

        public List<ObjectToChoose> Languages { get; }

        public List<ObjectToChoose> QuestionNumbers { get; }

        public List<ObjectToChoose> Categories { get; }

        public SessionSettings SessionSettings { get; }

        public bool CanStartLearning
        {
            get
            {
                return (SessionSettings.IsClosedChosen || SessionSettings.IsOpenChosen || SessionSettings.IsPronunciationChosen) &&
                    Categories.Count(x => x.IsChosen) > 0;
            }
        }

        public Func<double, string> GaugeLabelFormatter { get; }
        #endregion

        public LearnSettingsViewModel(SessionType? sessionType, SessionSettings sessionSettings)
        {
            SessionType = sessionType;
            LanguageClickCommand = new RelayCommand(LanguageClick);
            NumberClickCommand = new RelayCommand(NumberClick);
            CategoryClickCommand = new RelayCommand(CategoryClick);
            ClosedClickCommand = new RelayCommand(ClosedClick);
            OpenClickCommand = new RelayCommand(OpenClick);
            PronunciationClickCommand = new RelayCommand(PronunciationClick);
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

            QuestionNumbers = new List<ObjectToChoose>();
            QuestionNumbers.Add(new ObjectToChoose() { Object = (uint) 5 });
            QuestionNumbers.Add(new ObjectToChoose() { Object = (uint) 10 });
            QuestionNumbers.Add(new ObjectToChoose() { Object = (uint) 20 });

            var categories = CategoriesService.GetInstance().Categories.FindAll(x => x.LanguageId == id);
            Categories = new List<ObjectToChoose>();

            foreach(var category in categories)
            {
                Categories.Add(new ObjectToChoose()
                {
                    Object = category
                });
            }

            if (sessionSettings == null)
            {
                SessionSettings = new SessionSettings();
                Languages[0].IsChosen = true;
                QuestionNumbers[0].IsChosen = true;
            }
            else
            {
                SessionSettings = sessionSettings;

                Languages.First(x => (x.Object as LanguageName).LanguageId == SessionSettings.LanguageId).IsChosen = true;
                QuestionNumbers.First(x => (uint) x.Object == SessionSettings.NumberOfQuestions).IsChosen = true;

                foreach(var category in Categories)
                {
                    category.IsChosen = SessionSettings.CategoriesIds.Contains((category.Object as CategoryName).CategoryId);
                }
            }

            GaugeLabelFormatter = value => value.ToString("0.##") + " %";
        }

        private void ClosedClick(object obj)
        {
            SessionSettings.IsClosedChosen = !SessionSettings.IsClosedChosen;
            OnPropertyChanged("CanStartLearning");
        }

        private void OpenClick(object obj)
        {
            SessionSettings.IsOpenChosen = !SessionSettings.IsOpenChosen;
            OnPropertyChanged("CanStartLearning");
        }

        private void PronunciationClick(object obj)
        {
            SessionSettings.IsPronunciationChosen = !SessionSettings.IsPronunciationChosen;
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
            SessionSettings.LanguageId = (Languages.First(x => x.IsChosen).Object as LanguageName).LanguageId;
            SessionSettings.NumberOfQuestions = (uint)QuestionNumbers.First(x => x.IsChosen).Object;
            SessionSettings.CategoriesIds.Clear();

            List<uint> categoriesIds = new List<uint>();
            foreach (var category in Categories)
            {
                if (category.IsChosen)
                {
                    SessionSettings.CategoriesIds.Add((category.Object as CategoryName).Category.Id);
                }
            }

            if (SessionType != null)
            {
                // utworzenie sesji
                var session = await Task.Run(() => SessionsService.CreateSessionAsync(Settings.GetInstance().InterfaceLanguageId, SessionSettings.LanguageId, SessionType.Value, SessionSettings.NumberOfQuestions));

                if (session != null)
                {
                    // dodanie wybranych kategori do utworzonej sesji
                    foreach (var id in categoriesIds)
                    {
                        await Task.Run(() => SelectedCategoriesService.CreateSelectedCategoryAsync(session.Id, id));
                    }

                    if (SessionType == Enums.SessionType.TEST)
                    {
                        Configuration.GetInstance().TestControl = new LearnControl(session, SessionSettings);
                        Configuration.GetInstance().CurrentView = Configuration.GetInstance().TestControl;
                    }
                    else
                    {
                        Configuration.GetInstance().LearnControl = new LearnControl(session, SessionSettings);
                        Configuration.GetInstance().CurrentView = Configuration.GetInstance().LearnControl;
                    }
                }
            }
            else
            {
                (Configuration.GetInstance().SettingsControl.DataContext as SettingsViewModel)
                    .CustomizingSchedule.SessionSettings = SessionSettings;
                Settings.Store();
                Configuration.GetInstance().CurrentView = Configuration.GetInstance().SettingsControl;
            }
        }
    }
}
