using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Controls;
using LangApp.WpfClient.Views.Windows;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class LearnViewModel : NotifyPropertyChanged
    {
        #region Constants
        private static readonly int SAMPLE_RATE = 44100;
        private static readonly int BITS_PER_SAMPLE = 16;
        private static readonly int AUDIO_CHANNELS = 2;
        #endregion

        #region Commands
        public ICommand ExitCommand { get; }
        public ICommand SkipCommand { get; }
        public ICommand CheckCommand { get; }
        public ICommand ShowAnswerCommand { get; }
        public ICommand ClosedAnswerCheckedCommand { get; }
        public ICommand RecordCommand { get; }
        public ICommand RecordPlayCommand { get; }
        public ICommand StarMouseLeftButtonDownCommand { get; }
        public ICommand AnswerVolumeMouseLeftButtonDownCommand { get; }
        #endregion

        #region Properties
        public bool IsTest { get; }

        public SessionSettings SessionSettings { get; }

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

        private QuestionType _questionType;
        public QuestionType QuestionType
        {
            get
            {
                return _questionType;
            }
            set
            {
                _questionType = value;
                OnPropertyChanged();
                OnPropertyChanged("ClosedAnswerVisibility");
                OnPropertyChanged("OpenAnswerVisibility");
                OnPropertyChanged("PronunciationAnswerVisibility");

                if(_questionType == QuestionType.CLOSED)
                {
                    PrepareClosedAnswers();
                }
            }
        }

        public Visibility ClosedAnswerVisibility
        {
            get
            {
                if (_questionType == QuestionType.CLOSED)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility OpenAnswerVisibility
        {
            get
            {
                if (_questionType == QuestionType.OPEN)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility PronunciationAnswerVisibility
        {
            get
            {
                if (_questionType == QuestionType.PRONUNCIATION)
                    return Visibility.Visible;

                return Visibility.Collapsed;
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

        private bool _isAnswerVisible;
        public bool IsAnswerVisible
        {
            get
            {
                return _isAnswerVisible;
            }
            set
            {
                _isAnswerVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _canGoToFurther = true;
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
                OnPropertyChanged("IsCheckButtonEnabled");
                OnPropertyChanged("IsRecordButtonEnabled");
                OnPropertyChanged("AreAnswersEnabled");
            }
        }

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
                TimeSpan timeSpan = DateTime.Now - _startTime;
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

        private bool _isRecording;
        public bool IsRecording
        {
            get
            {
                return _isRecording;
            }
            set
            {
                _isRecording = value;
                OnPropertyChanged();
                OnPropertyChanged("IsCheckButtonEnabled");
            }
        }

        private bool _isRecordComplete = true;
        public bool IsRecordComplete
        {
            get
            {
                return _isRecordComplete;
            }
            set
            {
                _isRecordComplete = value;
                OnPropertyChanged();
            }
        }

        private int _recordProgress;
        public int RecordProgress
        {
            get
            {
                return _recordProgress;
            }
            set
            {
                _recordProgress = value;
                OnPropertyChanged();
            }
        }

        private bool _isRecordPlaying;
        public bool IsRecordPlaying
        {
            get
            {
                return _isRecordPlaying;
            }
            set
            {
                _isRecordPlaying = value;
                OnPropertyChanged();
                OnPropertyChanged("IsRecordButtonEnabled");
            }
        }

        private bool _isRecordPlayComplete = true;
        public bool IsRecordPlayComplete
        {
            get
            {
                return _isRecordPlayComplete;
            }
            set
            {
                _isRecordPlayComplete = value;
                OnPropertyChanged();
            }
        }

        private double _recordPlayProgress;
        public double RecordPlayProgress
        {
            get
            {
                return _recordPlayProgress;
            }
            set
            {
                _recordPlayProgress = value;
                OnPropertyChanged();
            }
        }

        private bool _isPlayButtonEnabled;
        public bool IsPlayButtonEnabled
        {
            get
            {
                return _isPlayButtonEnabled;
            }
            set
            {
                _isPlayButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isProcessingPronunciation;
        public bool IsProcessingAnswer
        {
            get
            {
                return _isProcessingPronunciation;
            }
            set
            {
                _isProcessingPronunciation = value;
                OnPropertyChanged("IsCheckButtonEnabled");
            }
        }

        public bool IsCheckButtonEnabled
        {
            get
            {
                return (!IsRecording || CanGoFurther) && !IsProcessingAnswer;
            }
        }

        private bool _isAnswerPlaying;
        public bool IsAnswerPlaying
        {
            get
            {
                return _isAnswerPlaying;
            }
            set
            {
                _isAnswerPlaying = value;
                OnPropertyChanged();
            }
        }

        public bool IsRecordButtonEnabled
        {
            get
            {
                return !IsRecordPlaying && AreAnswersEnabled;
            }
        }

        public bool AreAnswersEnabled
        {
            get
            {
                return IsTest || !CanGoFurther;
            }
        }
        #endregion

        #region Variables
        private Session _session;
        private Language _language;

        private BilingualDictionary _dictionary;
        private Random _random;
        private List<uint> _previousWordsIds;
        private List<Answer> _answers;
        private string _pronunciationResult;
        private int _properClosedAnswerIndex;
        private int _selectedClosedAnswerIndex;
        private int _selectedCategoriesWordCount;

        private DispatcherTimer _dispatcherGeneralTimer;
        private DispatcherTimer _dispatcherRecordTimer;
        private DispatcherTimer _dispatcherRecordPlayTimer;
        private DateTime _startTime;
        private DateTime _questionAppearedTime;

        private WaveIn _waveIn;
        private MemoryStream _waveMemoryStream;
        private WaveFileWriter _waveFileWriter;
        private PushAudioInputStream _audioInputStream;
        private SoundPlayer _soundPlayer;
        private double _recordPlayValueToAdd;
        #endregion

        public LearnViewModel(Session session, SessionSettings sessionSettings)
        {
            _session = session;
            _language = LanguagesService.GetInstance().Languages.First(x => x.Id == sessionSettings.LanguageId);
            IsTest = session.Type == SessionType.TEST;
            SessionSettings = sessionSettings;

            _dictionary = TranslationsService.GetInstance().Dictionaries.First(x => 
                x.FirstLanguage.Id == LanguagesService.GetInstance().Languages[0].Id &&
                x.SecondLanguage.Id == _language.Id);
            _random = new Random();
            _previousWordsIds = new List<uint>();
            _answers = new List<Answer>();

            foreach(var pair in _dictionary.Dictionary)
            {
                if(SessionSettings.CategoriesIds.Contains(pair.Key.CategoryId))
                {
                    _selectedCategoriesWordCount++;
                }
            }

            /// timery
            _startTime = DateTime.Now;

            _dispatcherGeneralTimer = new DispatcherTimer();
            _dispatcherGeneralTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherGeneralTimer.Tick += (sender, e) => 
            {
                OnPropertyChanged("Timer");
            };
            _dispatcherGeneralTimer.Start();

            _dispatcherRecordTimer = new DispatcherTimer();
            _dispatcherRecordTimer.Interval = TimeSpan.FromMilliseconds(40);
            _dispatcherRecordTimer.Tick += (sender, e) =>
            {
                RecordProgress++;

                if(_recordProgress >= 100)
                {
                    StopRecord();
                }
            };

            _dispatcherRecordPlayTimer = new DispatcherTimer();
            _dispatcherRecordPlayTimer.Interval = TimeSpan.FromMilliseconds(40);
            _dispatcherRecordPlayTimer.Tick += (sender, e) =>
            {
                RecordPlayProgress += _recordPlayValueToAdd;

                if (_recordPlayProgress >= 100)
                {
                    StopRecordPlay();
                }
            };
            ///

            /// nagrywanie dzwięku
            _waveIn = new WaveIn();
            _waveIn.DeviceNumber = 0; // domyślny mikrofon
            _waveIn.DataAvailable += (sender, e) =>
            {
                _waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
                _audioInputStream.Write(e.Buffer);
            };
            _waveIn.WaveFormat = new WaveFormat(SAMPLE_RATE, BITS_PER_SAMPLE, AUDIO_CHANNELS);

            _audioInputStream = AudioInputStream.CreatePushStream(
                AudioStreamFormat.GetWaveFormatPCM((uint)SAMPLE_RATE, (byte)BITS_PER_SAMPLE, (byte)AUDIO_CHANNELS));

            _soundPlayer = new SoundPlayer();
            ///

            ExitCommand = new RelayCommand(Exit);
            SkipCommand = new RelayCommand(Skip);
            CheckCommand = new RelayCommand(Check);
            ShowAnswerCommand = new RelayCommand(ShowAnswer);
            ClosedAnswerCheckedCommand = new RelayCommand(ClosedAnswerChecked);
            RecordCommand = new RelayCommand(Record);
            RecordPlayCommand = new RelayCommand(RecordPlay);
            StarMouseLeftButtonDownCommand = new RelayCommand(StarMouseLeftButtonDown);
            AnswerVolumeMouseLeftButtonDownCommand = new RelayCommand(AnswerVolumeMouseLeftButtonDown);
            
            GetNextQuestion();
        }

        private void Exit(object obj)
        {
            ConfirmationWindow confirmationWindow;

            if (IsTest)
            {
                confirmationWindow = new ConfirmationWindow(Application.Current.Resources["finish"].ToString(),
                    Application.Current.Resources["test_finish_confirmation"].ToString());
            }
            else
            {
                confirmationWindow = new ConfirmationWindow(Application.Current.Resources["finish"].ToString(),
                    Application.Current.Resources["learn_finish_confirmation"].ToString());
            }
                
            confirmationWindow.ShowDialog();

            if (confirmationWindow.DialogResult == true)
            {
                if (!CanGoFurther)
                {
                    QuestionCounter--;
                }

                Finish();
            }
        }

        private void Skip(object obj)
        {
            QuestionCounter--;
            GetNextQuestion();
        }

        private async void Check(object obj)
        {
            IsProcessingAnswer = true;
            Mouse.OverrideCursor = Cursors.AppStarting;

            try
            {
                if (CanGoFurther)
                {
                    string userAnswer = null;

                    switch (_questionType)
                    {
                        case QuestionType.CLOSED:
                            userAnswer = _closedAnswers[_selectedClosedAnswerIndex];
                            break;

                        case QuestionType.OPEN:
                            userAnswer = _openAnswer.Trim();
                            break;

                        case QuestionType.PRONUNCIATION:
                            userAnswer = await GetPronunciationResult();
                            break;
                    }

                    // dodanie odpowiedzi
                    var answer = await Task.Run(() => AnswersService.CreateAnswerAsync(_session, TranslationPair.Key.Id, (uint)_questionCounter,
                        _questionType, userAnswer, TranslationPair.Value.SecondLanguageTranslation.Value, DateTime.Now - _questionAppearedTime));

                    if (answer != null)
                    {
                        _answers.Add(answer);
                        GetNextQuestion();
                    }
                }
                else
                {
                    Configuration.GetInstance().NoConnection = false;

                    if (await IsAnswerCorrect())
                    {
                        CorrectMessage = Application.Current.Resources["correct_answer"].ToString();
                        CanGoFurther = true;
                    }
                    else
                    {
                        IncorrectMessage = Application.Current.Resources["incorrect_answer"].ToString();
                    }
                }
            }
            catch
            {
                Configuration.GetInstance().NoConnection = true;
            }

            Mouse.OverrideCursor = null;
            IsProcessingAnswer = false;
        }

        private async Task<string> GetPronunciationResult()
        {
            if (_waveFileWriter == null)
            {
                return "-";
            }

            if (_pronunciationResult == null)
            {
                string result = await Task.Run(() => SpeechToTextService.GetText(_audioInputStream, _language));
                _pronunciationResult = result.Replace(".", "").ToLowerInvariant();

                if(_pronunciationResult == string.Empty)
                {
                    _pronunciationResult = "-";
                }
            }

            return _pronunciationResult;
        }

        private async Task<bool> IsAnswerCorrect()
        {
            switch(_questionType)
            {
                case QuestionType.CLOSED:
                    return _properClosedAnswerIndex == _selectedClosedAnswerIndex;

                case QuestionType.OPEN:
                    return _openAnswer != null &&
                        TranslationPair.Value.SecondLanguageTranslation.Value == _openAnswer.Trim();

                case QuestionType.PRONUNCIATION:
                    return TranslationPair.Value.SecondLanguageTranslation.Value == await GetPronunciationResult();
            }

            return false;
        }

        private void ShowAnswer(object obj)
        {
            IsAnswerVisible = true;
        }

        private void ClosedAnswerChecked(object obj)
        {
            _selectedClosedAnswerIndex = int.Parse((string) obj);
        }

        private void GetNextQuestion()
        {
            if(QuestionCounter < SessionSettings.NumberOfQuestions)
            {
                /// przygotowanie interfejsu
                QuestionCounter++;
                CorrectMessage = string.Empty;
                IncorrectMessage = string.Empty;
                OpenAnswer = string.Empty;
                IsFirstClosedAnswerChecked = true;
                _selectedClosedAnswerIndex = 0;
                _pronunciationResult = null;
                _questionAppearedTime = DateTime.Now;
                IsAnswerVisible = false;

                if (!IsTest)
                {
                    CanGoFurther = false;
                }

                if(IsRecording)
                {
                    StopRecord();
                }

                if(IsRecordPlaying)
                {
                    StopRecordPlay();
                }

                IsPlayButtonEnabled = false;
                ///

                // losowanie pytania
                SetTranslationPair();

                // losowanie typu pytania
                SetQuestionType();
            }
            else
            {
                Finish();
            }
        }

        private void SetTranslationPair()
        {
            // index wylosowanej kategorii
            var caregoryIndex = _random.Next(0, SessionSettings.CategoriesIds.Count);

            // słowa należących do wylosowanej kategorii
            var words = _dictionary.Dictionary.Where(x => x.Key.CategoryId == SessionSettings.CategoriesIds[caregoryIndex]).ToList();

            // index wylosowanego słowa
            int wordIndex;

            if (_random.Next(0, 2) == 0)
            {
                // liczba słów z najmniejszą liczbą poprawnych odpowiedzi
                int minCount;

                // uporządkowanie rosnąco względem liczby poprawnych odpowiedzi
                if (_session.Type == SessionType.LEARN)
                {
                    words = words.OrderBy(x => x.Value.LearnCount).ToList();
                    minCount = words.FindIndex(x => x.Value.LearnCount > words[0].Value.LearnCount) + 1;
                }
                else
                {
                    words = words.OrderBy(x => x.Value.TestCount).ToList();
                    minCount = words.FindIndex(x => x.Value.TestCount > words[0].Value.TestCount) + 1;
                }

                wordIndex = _random.Next(0, minCount);
            }
            else
            {
                // losowanie słowa wśród wszystkich słów
                wordIndex = _random.Next(0, words.Count);
            }

            if(_previousWordsIds.Contains(words[wordIndex].Key.Id))
            {
                // jeśli słowo pojawiło się już wcześniej w obrębie rozgrywki to losujemy od nowa
                SetTranslationPair();
            }
            else
            {
                TranslationPair = words[wordIndex];
                _previousWordsIds.Add(words[wordIndex].Key.Id);

                if(_previousWordsIds.Count == _selectedCategoriesWordCount)
                {
                    // jeśli wyczerpaliśmy wszystkie słowa z wybranych kategorii
                    // to pozwalamy na ponowne wylosowanie pierwszej połowy z nich
                    _previousWordsIds.RemoveRange(0, _selectedCategoriesWordCount / 2 + 1);
                }
            }
        }

        private void SetQuestionType()
        {
            int questionTypeIndex;

            if (SessionSettings.IsClosedChosen && SessionSettings.IsOpenChosen && SessionSettings.IsPronunciationChosen)
            {
                questionTypeIndex = _random.Next(0, 3);
                QuestionType = (QuestionType)questionTypeIndex;
            }
            else if (SessionSettings.IsClosedChosen && SessionSettings.IsOpenChosen)
            {
                questionTypeIndex = _random.Next(0, 2);

                if (questionTypeIndex == 0)
                {
                    QuestionType = QuestionType.CLOSED;
                }
                else
                {
                    QuestionType = QuestionType.OPEN;
                }
            }
            else if (SessionSettings.IsClosedChosen && SessionSettings.IsPronunciationChosen)
            {
                questionTypeIndex = _random.Next(0, 2);

                if (questionTypeIndex == 0)
                {
                    QuestionType = QuestionType.CLOSED;
                }
                else
                {
                    QuestionType = QuestionType.PRONUNCIATION;
                }
            }
            else if (SessionSettings.IsOpenChosen && SessionSettings.IsPronunciationChosen)
            {
                questionTypeIndex = _random.Next(0, 2);

                if (questionTypeIndex == 0)
                {
                    QuestionType = QuestionType.OPEN;
                }
                else
                {
                    QuestionType = QuestionType.PRONUNCIATION;
                }
            }
            else if (SessionSettings.IsClosedChosen)
            {
                QuestionType = QuestionType.CLOSED;
            }
            else if (SessionSettings.IsOpenChosen)
            {
                QuestionType = QuestionType.OPEN;
            }
            else
            {
                QuestionType = QuestionType.PRONUNCIATION;
            }
        }

        private void PrepareClosedAnswers()
        {
            // index kategorii wylosowanego słowa
            var caregoryId = TranslationPair.Key.CategoryId;

            // słowa należące do kategorii
            var words = _dictionary.Dictionary.Where(x => x.Key.CategoryId == caregoryId).ToList();

            var closedAnswersWordsIds = new List<uint>() { TranslationPair.Key.Id };
            var closedAnswers = new string[4];
            closedAnswers[0] = TranslationPair.Value.SecondLanguageTranslation.Value;
            KeyValuePair<Word, TranslationSet> translationPair;

            for (int i = 1; i < 4; i++)
            {
                do
                {
                    int index = _random.Next(0, words.Count);
                    translationPair = words.ElementAt(index);
                } while (closedAnswersWordsIds.Contains(translationPair.Key.Id));

                closedAnswersWordsIds.Add(translationPair.Key.Id);
                closedAnswers[i] = translationPair.Value.SecondLanguageTranslation.Value;
            }

            ClosedAnswers = closedAnswers.OrderBy(x => x).ToArray();
            _properClosedAnswerIndex = Array.IndexOf(_closedAnswers, TranslationPair.Value.SecondLanguageTranslation.Value);
        }

        private void Finish()
        {
            Configuration.GetInstance().LearnFinishControl = new LearnFinishControl(_session, DateTime.Now - _startTime, _answers);
            Configuration.GetInstance().CurrentView = Configuration.GetInstance().LearnFinishControl;

            if (IsTest)
            {
                Configuration.GetInstance().TestControl = null;
            }
            else
            {
                Configuration.GetInstance().LearnControl = null;
            }

            _dispatcherGeneralTimer.Stop();
        }

        private void Record(object obj)
        {
            if (!IsRecording)
            {
                IsRecording = true;
                IsRecordComplete = false;
                RecordProgress = 0;
                IsPlayButtonEnabled = false;
                _pronunciationResult = null;

                var waveFormat = new WaveFormat(SAMPLE_RATE, BITS_PER_SAMPLE, AUDIO_CHANNELS);
                _waveMemoryStream = new MemoryStream();
                _waveFileWriter = new WaveFileWriter(new IgnoreDisposeStream(_waveMemoryStream), waveFormat);

                _dispatcherRecordTimer.Start();
                _waveIn.StartRecording();
            }
            else
            {
                StopRecord();
            }
        }

        private void StopRecord()
        {
            _dispatcherRecordTimer.Stop();
            _waveIn.StopRecording();

            IsRecording = false;
            IsRecordComplete = true;
            IsPlayButtonEnabled = true;

            _recordPlayValueToAdd = 100 / (_waveFileWriter.TotalTime.TotalMilliseconds / _dispatcherRecordPlayTimer.Interval.TotalMilliseconds);

            _waveFileWriter.Close();
            _audioInputStream.Close();
            _waveMemoryStream.Position = 0;
            _soundPlayer.Stream = _waveMemoryStream;
            _soundPlayer.LoadAsync();
        }

        private void RecordPlay(object obj)
        {
            if (!IsRecordPlaying)
            {
                RecordPlayProgress = 0;
                IsRecordPlaying = true;
                IsRecordPlayComplete = false;

                _dispatcherRecordPlayTimer.Start();
                _soundPlayer.Play();
            }
            else
            {
                StopRecordPlay();
            }
        }

        private void StopRecordPlay()
        {
            IsRecordPlaying = false;
            IsRecordPlayComplete = true;

            _dispatcherRecordPlayTimer.Stop();
            _soundPlayer.Stop();
        }

        private async void StarMouseLeftButtonDown(object obj)
        {
            var pair = obj as KeyValuePair<Word, TranslationSet>?;
            if (pair != null)
            {
                if (pair.Value.Value.IsFavourite)
                {
                    await FavouriteWordsService.RemoveFavouriteWordAsync(pair.Value.Value.FavouriteWordId.Value);
                }
                else
                {
                    await FavouriteWordsService.CreateFavouriteWordAsync
                        (pair.Value.Value.FirstLanguageTranslation.Id,
                        pair.Value.Value.SecondLanguageTranslation.Id);
                }
            }
        }

        private async void AnswerVolumeMouseLeftButtonDown(object obj)
        {
            if (!_isAnswerPlaying)
            {
                IsAnswerPlaying = true;
                await Task.Run(() => PronunciationsService.GetInstance().PlayPronunciation(TranslationPair.Value.SecondLanguageTranslation));
                IsAnswerPlaying = false;
            }
        }
    }
}
