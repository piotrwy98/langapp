using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class LearnViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand ExitCommand { get; set; }

        public ICommand SkipCommand { get; set; }

        public ICommand CheckCommand { get; set; }

        public ICommand ShowAnswerCommand { get; set; }

        public ICommand ClosedAnswerCheckedCommand { get; set; }
        #endregion

        #region Properties
        private KeyValuePair<Word, TranslationSet> _translationPair;
        public KeyValuePair<Word, TranslationSet> TranslationPair
        {
            get
            {
                return _translationPair;
            }
            set
            {
                _translationPair = value;
                OnPropertyChanged();
            }
        }

        private string _openAnswer;
        public string OpenAnswer
        {
            get
            {
                return _openAnswer;
            }
            set
            {
                _openAnswer = value;
                OnPropertyChanged();
            }
        }

        private string[] _closedAnswers;
        public string[] ClosedAnswers
        {
            get
            {
                return _closedAnswers;
            }
            set
            {
                _closedAnswers = value;
                OnPropertyChanged();
            }
        }

        private string _correctMessage;
        public string CorrectMessage
        {
            get
            {
                return _correctMessage;
            }
            set
            {
                _correctMessage = value;
                OnPropertyChanged();
                OnPropertyChanged("IsCorrectVisible");

                if(IsCorrectVisible)
                {
                    IncorrectMessage = string.Empty;
                }
            }
        }

        public bool IsCorrectVisible
        {
            get
            {
                return !string.IsNullOrEmpty(_correctMessage);
            }
        }

        private string _incorrectMessage;
        public string IncorrectMessage
        {
            get
            {
                return _incorrectMessage;
            }
            set
            {
                _incorrectMessage = value;
                OnPropertyChanged();
                OnPropertyChanged("IsIncorrectVisible");

                if (IsIncorrectVisible)
                {
                    CorrectMessage = string.Empty;
                }
            }
        }

        public bool IsIncorrectVisible
        {
            get
            {
                return !string.IsNullOrEmpty(_incorrectMessage);
            }
        }

        private bool _isShowAnswerVisible;
        public bool IsShowAnswerVisible
        {
            get
            {
                return _isShowAnswerVisible;
            }
            set
            {
                _isShowAnswerVisible = value;
                OnPropertyChanged();
                OnPropertyChanged("IsAnswerVisible");
            }
        }

        public bool IsAnswerVisible
        {
            get
            {
                return !_isShowAnswerVisible;
            }
        }

        private bool _canGoToFurther;
        public bool CanGoFurther
        {
            get
            {
                return _canGoToFurther;
            }
            set
            {
                _canGoToFurther = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfQuestions { get; } = 5;

        private int _questionCounter;
        public int QuestionCounter
        {
            get
            {
                return _questionCounter;
            }
            set
            {
                _questionCounter = value;
                OnPropertyChanged();
            }
        }

        public string Timer
        {
            get
            {
                TimeSpan timeSpan = _stopWatch.Elapsed;
                return string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
            }
        }

        private bool _isFirstClosedAnswerChecked;
        public bool IsFirstClosedAnswerChecked
        {
            get
            {
                return _isFirstClosedAnswerChecked;
            }
            set
            {
                _isFirstClosedAnswerChecked = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Variables
        private Guid _languageId;
        private List<Guid> _categoriesIds;
        private bool _isClosedChosen;
        private bool _isOpenChosen;
        private bool _isSpeakChosen;

        private BilingualDictionary _dictionary;
        private Random _random;
        private List<Guid> _previousWordsIds;
        private QuestionType _questionType;
        private DispatcherTimer _dispatcherTimer;
        private Stopwatch _stopWatch;
        private int _properClosedAnswerIndex;
        private int _selectedClosedAnswerIndex;
        #endregion

        public LearnViewModel(Guid languageId, List<Guid> categoriesIds, bool isClosedChosen, bool isOpenChosen, bool isSpeakChosen)
        {
            _languageId = languageId;
            _categoriesIds = categoriesIds;
            _isClosedChosen = isClosedChosen;
            _isOpenChosen = isOpenChosen;
            _isSpeakChosen = isSpeakChosen;

            _dictionary = TranslationsService.GetInstance().Dictionaries.First(x => 
                x.FirstLanguage.Id == LanguagesService.GetInstance().Languages[0].Id &&
                x.SecondLanguage.Id == _languageId);
            _random = new Random();
            _previousWordsIds = new List<Guid>();

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Start();

            _stopWatch = new Stopwatch();
            _stopWatch.Start();

            ExitCommand = new RelayCommand(Exit);
            SkipCommand = new RelayCommand(Skip);
            CheckCommand = new RelayCommand(Check);
            ShowAnswerCommand = new RelayCommand(ShowAnswer);
            ClosedAnswerCheckedCommand = new RelayCommand(ClosedAnswerChecked);

            GetNextQuestion();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            OnPropertyChanged("Timer");
        }

        private void Exit(object obj)
        {
            if(!CanGoFurther)
            {
                QuestionCounter--;
            }

            Finish();
        }

        private void Skip(object obj)
        {
            GetNextQuestion(true);
        }

        private void Check(object obj)
        {
            if(CanGoFurther)
            {
                GetNextQuestion();
            }
            else
            {
                if (IsAnswerCorrect())
                {
                    CorrectMessage = "Poprawne tłumaczenie";
                    CanGoFurther = true;
                }
                else
                {
                    IncorrectMessage = "Niepoprawne tłumaczenie";
                }
            }
        }

        private bool IsAnswerCorrect()
        {
            switch(_questionType)
            {
                case QuestionType.CLOSED:
                    return _properClosedAnswerIndex == _selectedClosedAnswerIndex;

                case QuestionType.OPEN:
                    return _openAnswer != null &&
                        TranslationPair.Value.SecondLanguageTranslation == _openAnswer.Trim().ToLowerInvariant();

                case QuestionType.PRONOUNCIATION:
                    return false;
            }

            return false;
        }

        private void ShowAnswer(object obj)
        {
            IsShowAnswerVisible = false;
        }

        private void ClosedAnswerChecked(object obj)
        {
            _selectedClosedAnswerIndex = int.Parse((string) obj);
        }

        private void GetNextQuestion(bool isSkipped = false)
        {
            if(QuestionCounter < NumberOfQuestions)
            {
                if(!isSkipped)
                {
                    QuestionCounter++;
                }

                CorrectMessage = string.Empty;
                IncorrectMessage = string.Empty;
                OpenAnswer = string.Empty;
                IsFirstClosedAnswerChecked = true;
                _selectedClosedAnswerIndex = 0;
                IsShowAnswerVisible = true;
                CanGoFurther = false;

                if (_previousWordsIds.Count == _dictionary.Dictionary.Count)
                {
                    _previousWordsIds.Clear();
                }

                do
                {
                    int index = _random.Next(0, _dictionary.Dictionary.Count);
                    TranslationPair = _dictionary.Dictionary.ElementAt(index);
                } while (_previousWordsIds.Contains(TranslationPair.Key.Id));

                ///
                var closedAnswersWords = new List<Guid>() { TranslationPair.Key.Id };
                var closedAnswers = new string[4];
                closedAnswers[0] = TranslationPair.Value.SecondLanguageTranslation;
                KeyValuePair<Word, TranslationSet> translationPair;

                for (int i = 1; i < 4; i++)
                {
                    do
                    {
                        int index = _random.Next(0, _dictionary.Dictionary.Count);
                        translationPair = _dictionary.Dictionary.ElementAt(index);
                    } while (closedAnswersWords.Contains(translationPair.Key.Id));

                    closedAnswersWords.Add(translationPair.Key.Id);
                    closedAnswers[i] = translationPair.Value.SecondLanguageTranslation;
                }

                ClosedAnswers = closedAnswers.OrderBy(x => x).ToArray();
                _properClosedAnswerIndex = Array.IndexOf(_closedAnswers, TranslationPair.Value.SecondLanguageTranslation);
                ///

                _previousWordsIds.Add(TranslationPair.Key.Id);
                _questionType = QuestionType.CLOSED;
            }
            else
            {
                Finish();
            }
        }

        private void Finish()
        {
            Configuration.GetInstance().CurrentView = new LearnFinishControl(Timer, QuestionCounter, NumberOfQuestions);
            _dispatcherTimer.Stop();
            _stopWatch.Reset();
        }

        public void Reset()
        {
            QuestionCounter = 0;
            _dispatcherTimer.Start();
            _stopWatch.Start();
            OnPropertyChanged("Timer");
            GetNextQuestion();
        }
    }
}
