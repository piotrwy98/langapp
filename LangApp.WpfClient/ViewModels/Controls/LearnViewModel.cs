﻿using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Controls;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

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
        public ICommand ExitCommand { get; set; }
        public ICommand SkipCommand { get; set; }
        public ICommand CheckCommand { get; set; }
        public ICommand ShowAnswerCommand { get; set; }
        public ICommand ClosedAnswerCheckedCommand { get; set; }
        public ICommand RecordCommand { get; set; }
        public ICommand RecordPlayCommand { get; set; }
        #endregion

        #region Properties
        public bool IsTest { get; }

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

        public bool IsCheckButtonEnabled
        {
            get
            {
                return !IsRecording || CanGoFurther;
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
        private List<Answer> _answers;
        private string _pronunciationResult;
        private int _properClosedAnswerIndex;
        private int _selectedClosedAnswerIndex;

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

        public LearnViewModel(bool isTest, Guid languageId, List<Guid> categoriesIds, bool isClosedChosen, bool isOpenChosen, bool isSpeakChosen)
        {
            IsTest = isTest;
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
            _answers = new List<Answer>();

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

            GetNextQuestion();
        }

        private void Exit(object obj)
        {
            if (!CanGoFurther)
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
                string userAnswer = null;

                switch(_questionType)
                {
                    case QuestionType.CLOSED:
                        userAnswer = _closedAnswers[_selectedClosedAnswerIndex];
                        break;

                    case QuestionType.OPEN:
                        userAnswer = _openAnswer.Trim();
                        break;

                    case QuestionType.PRONUNCIATION:
                        userAnswer = GetPronunciationResult();
                        break;
                }

                _answers.Add(new Answer()
                {
                    Index = QuestionCounter,
                    QuestionType = _questionType.GetName(),
                    UserAnswer = userAnswer,
                    CorrectAnswer = TranslationPair.Value.SecondLanguageTranslation,
                    IsAnswerCorrect = userAnswer == TranslationPair.Value.SecondLanguageTranslation,
                    Duration = DateTime.Now - _questionAppearedTime
                });

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

        private string GetPronunciationResult()
        {
            if(_pronunciationResult != null)
            {
                return _pronunciationResult;
            }

            string result = Task.Run(() => SpeechToTextService.GetText(_audioInputStream)).Result;
            _pronunciationResult = result.Replace(".", "").ToLowerInvariant();

            return _pronunciationResult;
        }

        private bool IsAnswerCorrect()
        {
            switch(_questionType)
            {
                case QuestionType.CLOSED:
                    return _properClosedAnswerIndex == _selectedClosedAnswerIndex;

                case QuestionType.OPEN:
                    return _openAnswer != null &&
                        TranslationPair.Value.SecondLanguageTranslation == _openAnswer.Trim();

                case QuestionType.PRONUNCIATION:
                    return TranslationPair.Value.SecondLanguageTranslation == GetPronunciationResult();
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
                _pronunciationResult = null;
                _questionAppearedTime = DateTime.Now;
                IsShowAnswerVisible = true;

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

                if (_previousWordsIds.Count == _dictionary.Dictionary.Count)
                {
                    _previousWordsIds.Clear();
                }

                do
                {
                    int index = _random.Next(0, _dictionary.Dictionary.Count);
                    TranslationPair = _dictionary.Dictionary.ElementAt(index);
                } while (_previousWordsIds.Contains(TranslationPair.Key.Id));

                _previousWordsIds.Add(TranslationPair.Key.Id);

                int questionTypeIndex;

                if(_isClosedChosen && _isOpenChosen && _isSpeakChosen)
                {
                    questionTypeIndex = _random.Next(0, 3);
                    QuestionType = (QuestionType) questionTypeIndex;
                }
                else if(_isClosedChosen && _isOpenChosen)
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
                else if (_isClosedChosen && _isSpeakChosen)
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
                else if (_isOpenChosen && _isSpeakChosen)
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
                else if(_isClosedChosen)
                {
                    QuestionType = QuestionType.CLOSED;
                }
                else if (_isOpenChosen)
                {
                    QuestionType = QuestionType.OPEN;
                }
                else
                {
                    QuestionType = QuestionType.PRONUNCIATION;
                }
            }
            else
            {
                Finish();
            }
        }

        private void PrepareClosedAnswers()
        {
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
        }

        private void Finish()
        {
            Configuration.GetInstance().LearnFinishControl = new LearnFinishControl(IsTest, DateTime.Now - _startTime, NumberOfQuestions, _answers);
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
    }
}