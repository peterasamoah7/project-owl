using Microsoft.CognitiveServices.Speech;
using ProjectOwl.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProjectOwl.Services
{
    public partial class SpeechService : ISpeechService
    {
        public SpeechService()
        {
        }

        public async Task<string> ExtractText(Stream stream)
        {
            var subkey = Environment.GetEnvironmentVariable("AzureSpeechServiceKey");
            var region = Environment.GetEnvironmentVariable("AzureSpeechServiceRegion");

            SpeechConfig speechConfig = SpeechConfig.FromSubscription(subkey, region);

            using var audioConfig = Helper.OpenWavFile(stream);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();

            return result.Text;
        }
    }
}
