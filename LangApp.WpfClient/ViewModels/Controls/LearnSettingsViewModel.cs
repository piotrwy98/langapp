using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using LangApp.WpfClient.Views.Controls;
using System;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class LearnSettingsViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand CategoryClickCommand { get; set; }

        public ICommand ClosedClickCommand { get; set; }

        public ICommand OpenClickCommand { get; set; }

        public ICommand SpeakClickCommand { get; set; }

        public ICommand StartLearningCommand { get; set; }
        #endregion

        #region Properties
        public List<CategoryToChoose> Categories { get; }

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

        public LearnSettingsViewModel()
        {
            CategoryClickCommand = new RelayCommand(CategoryClick);
            ClosedClickCommand = new RelayCommand(ClosedClick);
            OpenClickCommand = new RelayCommand(OpenClick);
            SpeakClickCommand = new RelayCommand(SpeakClick);
            StartLearningCommand = new RelayCommand(StartLearning);

            var categories = CategoriesService.GetInstance().Categories;
            Categories = new List<CategoryToChoose>();

            foreach(var category in categories)
            {
                Categories.Add(new CategoryToChoose()
                {
                    Category = category,
                    IsChosen = false
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

        private void CategoryClick(object obj)
        {
            var category = obj as CategoryToChoose;
            if(obj != null)
            {
                category.IsChosen = !category.IsChosen;
                OnPropertyChanged("CanStartLearning");
            }
        }

        private void StartLearning(object obj)
        {
            List<Guid> categoriesIds = new List<Guid>();
            foreach(var category in Categories)
            {
                if(category.IsChosen)
                {
                    categoriesIds.Add(category.Category.Id);
                }
            }

            Configuration.GetInstance().LearnControl = new LearnControl(LanguagesService.GetInstance().Languages[1].Id, categoriesIds, _isClosedChosen, _isOpenChosen, _isSpeakChosen);
            Configuration.GetInstance().CurrentView = Configuration.GetInstance().LearnControl;
        }
    }
}
