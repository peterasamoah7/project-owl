using Microsoft.CognitiveServices.Speech;
using ProjectOwl.Interfaces;
using ProjectOwl.Models;
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


        public async Task ContinuousRecognitionWithFileAsync(Stream stream, TextCapture textCapture)
        {
            // <recognitionContinuousWithFile>
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var subkey = Environment.GetEnvironmentVariable("AzureSpeechServiceKey");
            var region = Environment.GetEnvironmentVariable("AzureSpeechServiceRegion");

            var config = SpeechConfig.FromSubscription(subkey, region);
            //string text = string.Empty; 
            var stopRecognition = new TaskCompletionSource<int>();

            // Creates a speech recognizer using file as audio input.
            // Replace with your own audio file name.
            using var audioInput = Helper.OpenWavFile(stream);
            using var recognizer = new SpeechRecognizer(config, audioInput);
            // Subscribes to events.
            recognizer.Recognizing += (s, e) =>
            {
                //Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
            };

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    textCapture.Text += e.Result.Text;
                    //Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            recognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStarted += (s, e) =>
            {
                Console.WriteLine("\n    Session started event.");
            };

            recognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                Console.WriteLine("\nStop recognition.");
                stopRecognition.TrySetResult(0);
            };

            // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            // Waits for completion.
            // Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            // Stops recognition.
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            // </recognitionContinuousWithFile>
        }       
    }
}
