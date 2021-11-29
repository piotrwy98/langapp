﻿using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LiveCharts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Services
{
    public class AnswersService : HttpClientService
    {
        private static AnswersService _instace;

        public List<Answer> Answers { get; }

        #region Stats
        public List<ChartValues<ChartItem>> LearnDailyValues { get; private set; }
        public List<ChartValues<ChartItem>> TestDailyValues { get; private set; }
        public List<ChartValues<ChartItem>> PercentDailyValues { get; private set; }

        public List<ChartValues<ChartItem>> LearnMonthlyValues { get; private set; }
        public List<ChartValues<ChartItem>> TestMonthlyValues { get; private set; }
        public List<ChartValues<ChartItem>> PercentMonthlyValues { get; private set; }

        public List<ChartValues<ChartItem>> LearnYearlyValues { get; private set; }
        public List<ChartValues<ChartItem>> TestYearlyValues { get; private set; }
        public List<ChartValues<ChartItem>> PercentYearlyValues { get; private set; }
        #endregion

        private AnswersService()
        {
            Answers = (List<Answer>) GetAnswersAsync().Result;
            GenerateStats();
        }

        public static AnswersService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new AnswersService();
            }

            return _instace;
        }

        private async Task<IEnumerable<Answer>> GetAnswersAsync()
        {
            var response = await HttpClient.GetAsync("http://localhost:5000/answers").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Answer>>(json);
            }

            return null;
        }

        public static async Task<Answer> CreateAnswerAsync(uint sessionId, uint numberInSession, QuestionType questionType, string value, string correctAnswer, TimeSpan duration)
        {
            var answer = new Answer()
            {
                SessionId = sessionId,
                NumberInSession = numberInSession,
                QuestionType = questionType,
                Value = value,
                CorrectAnswer = correctAnswer,
                DurationMs = (uint) duration.TotalMilliseconds
            };

            var content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("http://localhost:5000/answers", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                answer = JsonConvert.DeserializeObject<Answer>(json);
                GetInstance().Answers.Add(answer);
                GetInstance().AddToStats(answer);

                return answer;
            }

            return null;
        }

        private void GenerateStats()
        {
            LearnDailyValues = LanguagesService.GetLanguageValues();
            TestDailyValues = LanguagesService.GetLanguageValues();
            PercentDailyValues = LanguagesService.GetLanguageValues();

            LearnMonthlyValues = LanguagesService.GetLanguageValues();
            TestMonthlyValues = LanguagesService.GetLanguageValues();
            PercentMonthlyValues = LanguagesService.GetLanguageValues();

            LearnYearlyValues = LanguagesService.GetLanguageValues();
            TestYearlyValues = LanguagesService.GetLanguageValues();
            PercentYearlyValues = LanguagesService.GetLanguageValues();

            int answerIndex = 0;
            var dateTime = Configuration.GetInstance().User.RegisterDateTime.Date;
            int languagesCount = LanguagesService.GetInstance().Languages.Count + 1;

            while (dateTime <= DateTime.Now)
            {
                // dodajemy dni
                for (int i = 0; i < languagesCount; i++)
                {
                    LearnDailyValues[i].Add(new ChartItem()
                    {
                        DateTime = dateTime,
                        Value = 0
                    });

                    TestDailyValues[i].Add(new ChartItem()
                    {
                        DateTime = dateTime,
                        Value = 0
                    });

                    PercentDailyValues[i].Add(new ChartItem()
                    {
                        DateTime = dateTime,
                        Value = 0
                    });
                }

                if (LearnMonthlyValues[0].Count < 1 || LearnMonthlyValues[0].Last().DateTime.Month != dateTime.Month)
                {
                    // dodajemy miesiące
                    for (int i = 0; i < languagesCount; i++)
                    {
                        LearnMonthlyValues[i].Add(new ChartItem()
                        {
                            DateTime = new DateTime(dateTime.Year, dateTime.Month, 1),
                            Value = 0
                        });

                        TestMonthlyValues[i].Add(new ChartItem()
                        {
                            DateTime = new DateTime(dateTime.Year, dateTime.Month, 1),
                            Value = 0
                        });

                        PercentMonthlyValues[i].Add(new ChartItem()
                        {
                            DateTime = new DateTime(dateTime.Year, dateTime.Month, 1),
                            Value = 0
                        });
                    }

                    if (LearnYearlyValues[0].Count < 1 || LearnYearlyValues[0].Last().DateTime.Year != dateTime.Year)
                    {
                        // dodajemy lata
                        for (int i = 0; i < languagesCount; i++)
                        {
                            LearnYearlyValues[i].Add(new ChartItem()
                            {
                                DateTime = new DateTime(dateTime.Year, 1, 1),
                                Value = 0
                            });

                            TestYearlyValues[i].Add(new ChartItem()
                            {
                                DateTime = new DateTime(dateTime.Year, 1, 1),
                                Value = 0
                            });

                            PercentYearlyValues[i].Add(new ChartItem()
                            {
                                DateTime = new DateTime(dateTime.Year, 1, 1),
                                Value = 0
                            });
                        }
                    }
                }

                Session session = null;

                // dodajemy wartości
                while (answerIndex < Answers.Count)
                {
                    if(session == null || session.Id != Answers[answerIndex].SessionId)
                    {
                        session = SessionsService.GetInstance().Sessions.First(x => x.Id == Answers[answerIndex].SessionId);

                        if (dateTime != session.StartDateTime.Date)
                        {
                            break;
                        }
                    }

                    if (session.FirstLanguageId == Settings.GetInstance().InterfaceLanguageId)
                    {
                        if (session.Type == SessionType.LEARN)
                        {
                            LearnDailyValues[0].Last().Value++;
                            LearnDailyValues[(int)session.SecondLanguageId].Last().Value++;

                            LearnMonthlyValues[0].Last().Value++;
                            LearnMonthlyValues[(int)session.SecondLanguageId].Last().Value++;

                            LearnYearlyValues[0].Last().Value++;
                            LearnYearlyValues[(int)session.SecondLanguageId].Last().Value++;
                        }
                        else
                        {
                            TestDailyValues[0].Last().Value++;
                            TestDailyValues[(int)session.SecondLanguageId].Last().Value++;

                            TestMonthlyValues[0].Last().Value++;
                            TestMonthlyValues[(int)session.SecondLanguageId].Last().Value++;

                            TestYearlyValues[0].Last().Value++;
                            TestYearlyValues[(int)session.SecondLanguageId].Last().Value++;

                            // zliczamy prawidłowe odpowiedzi
                            if(Answers[answerIndex].IsAnswerCorrect)
                            {
                                PercentDailyValues[0].Last().Value++;
                                PercentDailyValues[(int)session.SecondLanguageId].Last().Value++;

                                PercentMonthlyValues[0].Last().Value++;
                                PercentMonthlyValues[(int)session.SecondLanguageId].Last().Value++;

                                PercentYearlyValues[0].Last().Value++;
                                PercentYearlyValues[(int)session.SecondLanguageId].Last().Value++;
                            }
                        }
                    }

                    answerIndex++;
                }

                dateTime = dateTime.AddDays(1);
            }

            // obliczamy procent prawidłowych odpowiedzi
            for(int i = 0; i < languagesCount; i++)
            {
                for(int j = 0; j < PercentDailyValues[i].Count; j++)
                {
                    PercentDailyValues[i][j].Value *= 100 / SessionsService.GetInstance().TotalDailyAnswers[i][j].Value;
                }

                for (int j = 0; j < PercentMonthlyValues[i].Count; j++)
                {
                    PercentMonthlyValues[i][j].Value *= 100 / SessionsService.GetInstance().TotalMonthlyAnswers[i][j].Value;
                }

                for (int j = 0; j < PercentYearlyValues[i].Count; j++)
                {
                    PercentYearlyValues[i][j].Value *= 100 / SessionsService.GetInstance().TotalYearlyAnswers[i][j].Value;
                }

                // usuwamy puste wartości
                foreach (var chartItem in PercentDailyValues[i].Reverse())
                {
                    if (chartItem.Value is double.NaN)
                    {
                        PercentDailyValues[i].Remove(chartItem);
                    }
                }

                foreach (var chartItem in PercentMonthlyValues[i].Reverse())
                {
                    if (chartItem.Value is double.NaN)
                    {
                        PercentMonthlyValues[i].Remove(chartItem);
                    }
                }

                foreach (var chartItem in PercentYearlyValues[i].Reverse())
                {
                    if(chartItem.Value is double.NaN)
                    {
                        PercentYearlyValues[i].Remove(chartItem);
                    }
                }
            }
        }

        public void AddNewSession(Session session)
        {
            var dateTime = LearnDailyValues[0].Last().DateTime.AddDays(1);
            int languagesCount = LanguagesService.GetInstance().Languages.Count + 1;

            while (dateTime <= DateTime.Now)
            {
                // dodajemy dni
                for (int i = 0; i < languagesCount; i++)
                {
                    LearnDailyValues[i].Add(new ChartItem()
                    {
                        DateTime = dateTime,
                        Value = 0
                    });

                    TestDailyValues[i].Add(new ChartItem()
                    {
                        DateTime = dateTime,
                        Value = 0
                    });

                    PercentDailyValues[i].Add(new ChartItem()
                    {
                        DateTime = dateTime,
                        Value = 0
                    });
                }

                if (LearnMonthlyValues[0].Count < 1 || LearnMonthlyValues[0].Last().DateTime.Month != dateTime.Month)
                {
                    // dodajemy miesiące
                    for (int i = 0; i < languagesCount; i++)
                    {
                        LearnMonthlyValues[i].Add(new ChartItem()
                        {
                            DateTime = new DateTime(dateTime.Year, dateTime.Month, 1),
                            Value = 0
                        });

                        TestMonthlyValues[i].Add(new ChartItem()
                        {
                            DateTime = new DateTime(dateTime.Year, dateTime.Month, 1),
                            Value = 0
                        });

                        PercentMonthlyValues[i].Add(new ChartItem()
                        {
                            DateTime = new DateTime(dateTime.Year, dateTime.Month, 1),
                            Value = 0
                        });
                    }

                    if (LearnYearlyValues[0].Count < 1 || LearnYearlyValues[0].Last().DateTime.Year != dateTime.Year)
                    {
                        // dodajemy lata
                        for (int i = 0; i < languagesCount; i++)
                        {
                            LearnYearlyValues[i].Add(new ChartItem()
                            {
                                DateTime = new DateTime(dateTime.Year, 1, 1),
                                Value = 0
                            });

                            TestYearlyValues[i].Add(new ChartItem()
                            {
                                DateTime = new DateTime(dateTime.Year, 1, 1),
                                Value = 0
                            });

                            PercentYearlyValues[i].Add(new ChartItem()
                            {
                                DateTime = new DateTime(dateTime.Year, 1, 1),
                                Value = 0
                            });
                        }
                    }
                }
            }

            // liczymy na nowo procenty
            var dailyValue = 100 / (SessionsService.GetInstance().TotalDailyAnswers[0].Last().Value - session.QuestionsNumber);
            var dailyCorrectAnswers = PercentDailyValues[0].Last().Value / dailyValue;
            PercentDailyValues[0].Last().Value = 100 * dailyCorrectAnswers / SessionsService.GetInstance().TotalDailyAnswers[0].Last().Value;

            dailyValue = 100 / (SessionsService.GetInstance().TotalDailyAnswers[(int)session.SecondLanguageId].Last().Value - session.QuestionsNumber);
            dailyCorrectAnswers = PercentDailyValues[(int)session.SecondLanguageId].Last().Value / dailyValue;
            PercentDailyValues[(int)session.SecondLanguageId].Last().Value = 100 * dailyCorrectAnswers / SessionsService.GetInstance().TotalDailyAnswers[(int)session.SecondLanguageId].Last().Value;

            /*_totalMonthlyValues[0].Last().Value += session.QuestionsNumber;
            _totalMonthlyValues[(int)session.SecondLanguageId].Last().Value += session.QuestionsNumber;

            _totalYearlyValues[0].Last().Value += session.QuestionsNumber;
            _totalYearlyValues[(int)session.SecondLanguageId].Last().Value += session.QuestionsNumber;*/
        }

        private void AddToStats(Answer answer)
        {
            var session = SessionsService.GetInstance().Sessions.First(x => x.Id == answer.SessionId);

            if (session.Type == SessionType.LEARN)
            {
                LearnDailyValues[0].Last().Value++;
                LearnDailyValues[(int)session.SecondLanguageId].Last().Value++;

                LearnMonthlyValues[0].Last().Value++;
                LearnMonthlyValues[(int)session.SecondLanguageId].Last().Value++;

                LearnYearlyValues[0].Last().Value++;
                LearnYearlyValues[(int)session.SecondLanguageId].Last().Value++;
            }
            else
            {
                TestDailyValues[0].Last().Value++;
                TestDailyValues[(int)session.SecondLanguageId].Last().Value++;

                TestMonthlyValues[0].Last().Value++;
                TestMonthlyValues[(int)session.SecondLanguageId].Last().Value++;

                TestYearlyValues[0].Last().Value++;
                TestYearlyValues[(int)session.SecondLanguageId].Last().Value++;

                if(answer.IsAnswerCorrect)
                {
                    var dailyValue = 100 / SessionsService.GetInstance().TotalDailyAnswers[0].Last().Value;
                    var dailyCorrectAnswers = PercentDailyValues[0].Last().Value / dailyValue;
                    PercentDailyValues[0].Last().Value = dailyValue * (dailyCorrectAnswers + 1);

                    dailyValue = 100 / SessionsService.GetInstance().TotalDailyAnswers[(int)session.SecondLanguageId].Last().Value;
                    dailyCorrectAnswers = PercentDailyValues[(int)session.SecondLanguageId].Last().Value / dailyValue;
                    PercentDailyValues[(int)session.SecondLanguageId].Last().Value = dailyValue * (dailyCorrectAnswers + 1);
                }
            }
        }
    }
}