using LangApp.Shared.Models;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LangApp.WpfClient.Services
{
    public class PronunciationsService : HttpClientService
    {
        private static PronunciationsService _instace;

        public Dictionary<Translation, MemoryStream> StreamsDictionary { get; }

        private PronunciationsService()
        {
            StreamsDictionary = new Dictionary<Translation, MemoryStream>();
        }

        public static PronunciationsService GetInstance()
        {
            if (_instace == null)
            {
                _instace = new PronunciationsService();
            }

            return _instace;
        }

        public static async Task PlayPronunciation(Translation translation)
        {
            MemoryStream memoryStream;

            if(GetInstance().StreamsDictionary.ContainsKey(translation))
            {
                memoryStream = GetInstance().StreamsDictionary[translation];
            }
            else
            {
                _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Mouse.OverrideCursor = Cursors.AppStarting;
                }));

                memoryStream = await Task.Run(() => GetNewMemoryStream(translation));

                _ = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }

            if(memoryStream != null)
            {
                memoryStream.Position = 0;

                using (WaveStream blockAlignedStream =
                    new BlockAlignReductionStream(
                        WaveFormatConversionStream.CreatePcmStream(
                            new Mp3FileReader(memoryStream))))
                {
                    using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                    {
                        waveOut.Init(blockAlignedStream);
                        waveOut.Play();
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
            }
        }

        private static async Task<MemoryStream> GetNewMemoryStream(Translation translation)
        {
            var language = LanguagesService.GetInstance().Languages.FirstOrDefault(x => x.Id == translation.LanguageId);

            if (language == null)
            {
                return null;
            }

            var postRequest = new PostRequest()
            {
                Data = new PostRequestData()
                {
                    Text = translation.Value,
                    Voice = language.Code
                }
            };

            var serializerSettings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None
            };
            var content = new StringContent(JsonConvert.SerializeObject(postRequest, serializerSettings), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("https://api.soundoftext.com/sounds", content).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var id = JsonConvert.DeserializeObject<PostResponse>(json, serializerSettings).Id;
            var location = await GetLocation(id, serializerSettings);

            if(location == null)
            {
                return null;
            }

            var memoryStream = new MemoryStream();

            using (var responseStream = WebRequest.Create(location).GetResponse().GetResponseStream())
            {
                byte[] buffer = new byte[32768];
                int read;

                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }
            }

            GetInstance().StreamsDictionary.Add(translation, memoryStream);

            return memoryStream;
        }

        private async static Task<string> GetLocation(string id, JsonSerializerSettings serializerSettings)
        {
            var response = await HttpClient.GetAsync("https://api.soundoftext.com/sounds/" + id).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GetResponse>(json, serializerSettings);

            if(responseObject.Status == "Error")
            {
                return null;
            }

            if (responseObject.Status == "Pending")
            {
                System.Threading.Thread.Sleep(200);
                return await GetLocation(id, serializerSettings);
            }

            return responseObject.Location;
        }

        private class PostRequestData
        {
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("voice")]
            public string Voice { get; set; }
        }

        private class PostRequest
        {
            [JsonProperty("engine")]
            public string Engine { get; set; } = "Google";

            [JsonProperty("data")]
            public PostRequestData Data { get; set; }
        }

        private class PostResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }

        private class GetResponse
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("location")]
            public string Location { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
