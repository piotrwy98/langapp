using LangApp.Shared.Models;
using LangApp.WpfClient.Models;
using LiveCharts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static LangApp.Shared.Models.Enums;

namespace LangApp.WpfClient.Services
{
    public class SessionsService : HttpClientService
    {
        private static SessionsService _instace;

        public List<Session> Sessions { get; }

        #region Stats
        public List<ChartValues<ChartItem>> LearnDailyValues { get; private set; }
        public List<ChartValues<ChartItem>> TestDailyValues { get; private set; }
        public List<ChartValues<ChartItem>> TotalDailyAnswers { get; private set; }

        public List<ChartValues<ChartItem>> LearnMonthlyValues { get; private set; }
        public List<ChartValues<ChartItem>> TestMonthlyValues { get; private set; }
        public List<ChartValues<ChartItem>> TotalMonthlyAnswers { get; private set; }

        public List<ChartValues<ChartItem>> LearnYearlyValues { get; private set; }
        public List<ChartValues<ChartItem>> TestYearlyValues { get; private set; }
        public List<ChartValues<ChartItem>> TotalYearlyAnswers { get; private set; }
        #endregion

        private SessionsService()
        {
            Sessions = (List<Session>) GetSessionsAsync().Result;
            GenerateStats();
            Configuration.GetInstance().LearnSessionCounter = (uint) Sessions.Count(x => x.Type == SessionType.LEARN);
            Configuration.GetInstance().TestSessionCounter = (uint) (Sessions.Count - Configuration.GetInstance().LearnSessionCounter);
            Configuration.GetInstance().LastLearnSession = Sessions.LastOrDefault(x => x.Type == SessionType.LEARN)?.StartDateTime;
            Configuration.GetInstance().LastTestSession = Sessions.LastOrDefault(x => x.Type == SessionType.TEST)?.StartDateTime;
        }

        public static SessionsService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new SessionsService();
            }

            return _instace;
        }

        private async Task<IEnumerable<Session>> GetSessionsAsync()
        {
            var response = await HttpClient.GetAsync("http://localhost:5000/sessions").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Session>>(json);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await GetSessionsAsync();
            }

            return null;
        }

        public static async Task<Session> CreateSessionAsync(uint firstLangugeId, uint secondLangugeId, SessionType type, uint questionsNumber)
        {
            var session = new Session()
            {
                UserId = Configuration.User.Id,
                FirstLanguageId = firstLangugeId,
                SecondLanguageId = secondLangugeId,
                Type = type,
                QuestionsNumber = questionsNumber
            };

            var content = new StringContent(JsonConvert.SerializeObject(session), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("http://localhost:5000/sessions", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                session = JsonConvert.DeserializeObject<Session>(json);
                GetInstance().Sessions.Add(session);
                GetInstance().AddToStats(session);
                AnswersService.GetInstance().AddNewSession(session);

                if (session.Type == SessionType.LEARN)
                {
                    Configuration.GetInstance().LearnSessionCounter++;
                    Configuration.GetInstance().LastLearnSession = session.StartDateTime;
                }
                else
                {
                    Configuration.GetInstance().TestSessionCounter++;
                    Configuration.GetInstance().LastTestSession = session.StartDateTime;
                }

                return session;
            }
            else if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Configuration.RefreshToken();
                return await CreateSessionAsync(firstLangugeId, secondLangugeId, type, questionsNumber);
            }

            return null;
        }

        private void GenerateStats()
        {
            LearnDailyValues = LanguagesService.GetLanguageValues();
            TestDailyValues = LanguagesService.GetLanguageValues();
            TotalDailyAnswers = LanguagesService.GetLanguageValues();

            LearnMonthlyValues = LanguagesService.GetLanguageValues();
            TestMonthlyValues = LanguagesService.GetLanguageValues();
            TotalMonthlyAnswers = LanguagesService.GetLanguageValues();

            LearnYearlyValues = LanguagesService.GetLanguageValues();
            TestYearlyValues = LanguagesService.GetLanguageValues();
            TotalYearlyAnswers = LanguagesService.GetLanguageValues();

            int sessionIndex = 0;
            var dateTime = Configuration.User.RegisterDateTime.Date;
            int languagesCount = LanguagesService.GetInstance().Languages.Count + 1;

            while (dateTime <= DateTime.Now)
            {
                // dodajemy dni
                for(int i = 0; i < languagesCount; i++)
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

                    TotalDailyAnswers[i].Add(new ChartItem()
                    {
                        DateTime = dateTime,
                        Value = 0
                    });
                }

                if(LearnMonthlyValues[0].Count < 1 || LearnMonthlyValues[0].Last().DateTime.Month != dateTime.Month)
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

                        TotalMonthlyAnswers[i].Add(new ChartItem()
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

                            TotalYearlyAnswers[i].Add(new ChartItem()
                            {
                                DateTime = new DateTime(dateTime.Year, 1, 1),
                                Value = 0
                            });
                        }
                    }
                }

                // dodajemy wartości
                while (sessionIndex < Sessions.Count && dateTime == Sessions[sessionIndex].StartDateTime.Date)
                {
                    if(Sessions[sessionIndex].FirstLanguageId == Settings.GetInstance().InterfaceLanguageId)
                    {
                        if (Sessions[sessionIndex].Type == SessionType.LEARN)
                        {
                            LearnDailyValues[0].Last().Value++;
                            LearnDailyValues[(int)Sessions[sessionIndex].SecondLanguageId].Last().Value++;

                            LearnMonthlyValues[0].Last().Value++;
                            LearnMonthlyValues[(int)Sessions[sessionIndex].SecondLanguageId].Last().Value++;

                            LearnYearlyValues[0].Last().Value++;
                            LearnYearlyValues[(int)Sessions[sessionIndex].SecondLanguageId].Last().Value++;
                        }
                        else
                        {
                            TestDailyValues[0].Last().Value++;
                            TestDailyValues[(int)Sessions[sessionIndex].SecondLanguageId].Last().Value++;

                            TestMonthlyValues[0].Last().Value++;
                            TestMonthlyValues[(int)Sessions[sessionIndex].SecondLanguageId].Last().Value++;

                            TestYearlyValues[0].Last().Value++;
                            TestYearlyValues[(int)Sessions[sessionIndex].SecondLanguageId].Last().Value++;

                            TotalDailyAnswers[0].Last().Value += Sessions[sessionIndex].QuestionsNumber;
                            TotalDailyAnswers[(int)Sessions[sessionIndex].SecondLanguageId].Last().Value += Sessions[sessionIndex].QuestionsNumber;

                            TotalMonthlyAnswers[0].Last().Value += Sessions[sessionIndex].QuestionsNumber;
                            TotalMonthlyAnswers[(int)Sessions[sessionIndex].SecondLanguageId].Last().Value += Sessions[sessionIndex].QuestionsNumber;

                            TotalYearlyAnswers[0].Last().Value += Sessions[sessionIndex].QuestionsNumber;
                            TotalYearlyAnswers[(int)Sessions[sessionIndex].SecondLanguageId].Last().Value += Sessions[sessionIndex].QuestionsNumber;
                        }
                    }

                    sessionIndex++;
                }

                dateTime = dateTime.AddDays(1);
            }
        }

        private void AddToStats(Session session)
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

                    TotalDailyAnswers[i].Add(new ChartItem()
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

                        TotalMonthlyAnswers[i].Add(new ChartItem()
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

                            TotalYearlyAnswers[i].Add(new ChartItem()
                            {
                                DateTime = new DateTime(dateTime.Year, 1, 1),
                                Value = 0
                            });
                        }
                    }
                }
            }

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

                TotalDailyAnswers[0].Last().Value += session.QuestionsNumber;
                TotalDailyAnswers[(int)session.SecondLanguageId].Last().Value += session.QuestionsNumber;

                TotalMonthlyAnswers[0].Last().Value += session.QuestionsNumber;
                TotalMonthlyAnswers[(int)session.SecondLanguageId].Last().Value += session.QuestionsNumber;

                TotalYearlyAnswers[0].Last().Value += session.QuestionsNumber;
                TotalYearlyAnswers[(int)session.SecondLanguageId].Last().Value += session.QuestionsNumber;
            }
        }
    }
}
