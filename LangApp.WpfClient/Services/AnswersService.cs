using LangApp.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        private AnswersService()
        {
            Answers = (List<Answer>) GetAnswersAsync().Result;
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
                return answer;
            }

            return null;
        }
    }
}
