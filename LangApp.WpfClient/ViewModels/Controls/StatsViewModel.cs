using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LangApp.WpfClient.Services;
using LangApp.WpfClient.Views.Controls;
using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace LangApp.WpfClient.ViewModels.Controls
{
    public class StatsViewModel : NotifyPropertyChanged
    {
        #region Commands
        public ICommand PeriodClickCommand { get; }
        public ICommand LanguageClickCommand { get; }
        public ICommand SessionClickCommand { get; }
        #endregion

        #region Properties
        public List<ObjectToChoose> Periods { get; }
        public List<ObjectToChoose> Languages { get; }

        private double _axisMin;
        public double AxisMin
        {
            get 
            {
                return _axisMin; 
            }
            set
            {
                _axisMin = value;
                OnPropertyChanged();
            }
        }

        private double _axisMax;
        public double AxisMax
        {
            get 
            {
                return _axisMax; 
            }
            set
            {
                _axisMax = value;
                OnPropertyChanged();
            }
        }

        private Func<double, string> _dateTimeFormatter;
        public Func<double, string> DateTimeFormatter
        {
            get
            {
                return _dateTimeFormatter;
            }
            set
            {
                _dateTimeFormatter = value;
                OnPropertyChanged();
            }
        }

        public Func<double, string> PercentFormatter { get; }

        private double _axisStep;
        public double AxisStep
        {
            get
            {
                return _axisStep;
            }
            set
            {
                _axisStep = value;
                OnPropertyChanged();
            }
        }

        public double _axisUnit;
        public double AxisUnit
        {
            get
            {
                return _axisUnit;
            }
            set
            {
                _axisUnit = value;
                OnPropertyChanged();
            }
        }

        private ChartValues<ChartItem> _sessionsLearnValues;
        public ChartValues<ChartItem> SessionsLearnValues
        {
            get
            {
                return _sessionsLearnValues;
            }
            set
            {
                _sessionsLearnValues = value;
                OnPropertyChanged();
            }
        }

        private ChartValues<ChartItem> _sessionsTestValues;
        public ChartValues<ChartItem> SessionsTestValues
        {
            get
            {
                return _sessionsTestValues;
            }
            set
            {
                _sessionsTestValues = value;
                OnPropertyChanged();
            }
        }

        private ChartValues<ChartItem> _answersLearnValues;
        public ChartValues<ChartItem> AnswersLearnValues
        {
            get
            {
                return _answersLearnValues;
            }
            set
            {
                _answersLearnValues = value;
                OnPropertyChanged();
            }
        }

        private ChartValues<ChartItem> _answersTestValues;
        public ChartValues<ChartItem> AnswersTestValues
        {
            get
            {
                return _answersTestValues;
            }
            set
            {
                _answersTestValues = value;
                OnPropertyChanged();
            }
        }

        private ChartValues<ChartItem> _percentValues;
        public ChartValues<ChartItem> PercentValues
        {
            get
            {
                return _percentValues;
            }
            set
            {
                _percentValues = value;
                OnPropertyChanged();
            }
        }

        public List<Session> Sessions { get; }
        #endregion

        #region Variables
        private int _selectedLanguageId;
        private int _selectedPeriodId;
        #endregion

        public StatsViewModel()
        {
            PeriodClickCommand = new RelayCommand(PeriodClick);
            LanguageClickCommand = new RelayCommand(LanguageClick);
            SessionClickCommand = new RelayCommand(SessionClick);

            Periods = new List<ObjectToChoose>();
            Periods.Add(new ObjectToChoose()
            {
                Object = "dzienne",
                IsChosen = true
            });
            Periods.Add(new ObjectToChoose()
            {
                Object = "miesięczne"
            });
            Periods.Add(new ObjectToChoose()
            {
                Object = "roczne"
            });

            uint id = Settings.GetInstance().InterfaceLanguageId;
            var languages = LanguagesService.GetInstance().LanguageNames.FindAll(x => x.SourceLanguageId == id && x.LanguageId != id);
            Languages = new List<ObjectToChoose>();

            Languages.Add(new ObjectToChoose()
            {
                Object = new LanguageName()
                {
                    Value = "Wszystkie",
                    LanguageId = 0,
                    Language = new Language()
                    {
                        Id = 0,
                        ImagePath = "../../../Resources/Flags/earth.png"
                    }
                },
                IsChosen = true
            });

            foreach (var language in languages)
            {
                Languages.Add(new ObjectToChoose()
                {
                    Object = language
                });
            }

            var mapper = Mappers.Xy<ChartItem>()
               .X(model => model.DateTime.Ticks)
               .Y(model => model.Value);

            Charting.For<ChartItem>(mapper);

            PercentFormatter = value => value.ToString("N2") + " %";

            UpdateCharts();

            Sessions = SessionsService.GetInstance().Sessions.OrderByDescending(x => x.StartDateTime).ToList();
        }

        private void UpdateCharts()
        {
            var now = DateTime.Now;

            AxisMin = double.NaN;
            AxisMax = double.NaN;
            AxisStep = double.NaN;
            AxisUnit = double.NaN;

            switch (_selectedPeriodId)
            {
                case 0: // dzienne
                    SessionsLearnValues = SessionsService.GetInstance().LearnDailyValues[_selectedLanguageId];
                    SessionsTestValues = SessionsService.GetInstance().TestDailyValues[_selectedLanguageId];

                    AnswersLearnValues = AnswersService.GetInstance().LearnDailyValues[_selectedLanguageId];
                    AnswersTestValues = AnswersService.GetInstance().TestDailyValues[_selectedLanguageId];

                    PercentValues = AnswersService.GetInstance().PercentDailyValues[_selectedLanguageId];

                    AxisMin = now.Ticks - TimeSpan.FromDays(8).Ticks;
                    AxisMax = now.Ticks + TimeSpan.FromDays(1).Ticks;
                    DateTimeFormatter = value => new DateTime((long)value).ToString("dd.MM.yyyy");
                    AxisStep = TimeSpan.FromDays(1).Ticks;
                    AxisUnit = TimeSpan.TicksPerDay;
                    break;

                case 1: // miesięczne
                    SessionsLearnValues = SessionsService.GetInstance().LearnMonthlyValues[_selectedLanguageId];
                    SessionsTestValues = SessionsService.GetInstance().TestMonthlyValues[_selectedLanguageId];

                    AnswersLearnValues = AnswersService.GetInstance().LearnMonthlyValues[_selectedLanguageId];
                    AnswersTestValues = AnswersService.GetInstance().TestMonthlyValues[_selectedLanguageId];

                    PercentValues = AnswersService.GetInstance().PercentMonthlyValues[_selectedLanguageId];

                    AxisMin = now.Ticks - TimeSpan.FromDays(31 * 8).Ticks;
                    AxisMax = now.Ticks + TimeSpan.FromDays(31).Ticks;
                    DateTimeFormatter = value => new DateTime((long)value).ToString("MM.yyyy");
                    AxisStep = TimeSpan.FromDays(31).Ticks;
                    AxisUnit = TimeSpan.TicksPerDay * 30.79748;
                    break;

                case 2: // roczne
                    SessionsLearnValues = SessionsService.GetInstance().LearnYearlyValues[_selectedLanguageId];
                    SessionsTestValues = SessionsService.GetInstance().TestYearlyValues[_selectedLanguageId];

                    AnswersLearnValues = AnswersService.GetInstance().LearnYearlyValues[_selectedLanguageId];
                    AnswersTestValues = AnswersService.GetInstance().TestYearlyValues[_selectedLanguageId];

                    PercentValues = AnswersService.GetInstance().PercentYearlyValues[_selectedLanguageId];

                    AxisMin = now.Ticks - TimeSpan.FromDays(366 * 8).Ticks;
                    AxisMax = now.Ticks + TimeSpan.FromDays(366).Ticks;
                    DateTimeFormatter = value => new DateTime((long)value).ToString("yyyy");
                    AxisStep = TimeSpan.FromDays(366).Ticks;
                    AxisUnit = TimeSpan.TicksPerDay * 366.515;
                    break;
            }
        }

        private void PeriodClick(object obj)
        {
            var objectToChoose = obj as ObjectToChoose;
            if (objectToChoose != null && !objectToChoose.IsChosen)
            {
                foreach (var language in Periods)
                {
                    language.IsChosen = false;
                }

                objectToChoose.IsChosen = true;
                _selectedPeriodId = Periods.IndexOf(objectToChoose);
                UpdateCharts();
            }
        }

        private void LanguageClick(object obj)
        {
            var objectToChoose = obj as ObjectToChoose;
            if (objectToChoose != null && !objectToChoose.IsChosen)
            {
                foreach (var language in Languages)
                {
                    language.IsChosen = false;
                }

                objectToChoose.IsChosen = true;
                _selectedLanguageId = (int)(objectToChoose.Object as LanguageName).LanguageId;
                UpdateCharts();
            }
        }

        private void SessionClick(object obj)
        {
            var session = obj as Session;
            if(session != null)
            {
                Configuration.GetInstance().CurrentView = new LearnDetailsControl(session, null);
            }
        }
    }
}
