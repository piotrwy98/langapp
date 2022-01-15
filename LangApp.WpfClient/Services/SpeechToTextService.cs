using LangApp.Shared.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;

namespace LangApp.WpfClient.Services
{
    public class SpeechToTextService
    {
        private static readonly SpeechConfig _speechConfig = SpeechConfig.FromSubscription("135c0e82552d496b8232545dced12ccd", "northeurope");

        public static string GetText(AudioInputStream audioInputStream, Language language)
        {
            using (var audioConfig = AudioConfig.FromStreamInput(audioInputStream))
            {
                using (var recognizer = new SpeechRecognizer(_speechConfig, language.Code, audioConfig))
                {
                    var response = recognizer.RecognizeOnceAsync().Result;

                    if(response.Reason == ResultReason.Canceled)
                    {
                        throw new Exception();
                    }

                    return response.Text;
                }
            }
        }
    }
}
