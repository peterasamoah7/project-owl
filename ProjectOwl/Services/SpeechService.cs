/**
 * Asynchronous Conversation Transcription using Microsoft Speech SDK C#
 * References and inspired by approach used in these examples
 * https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-async-conversation-transcription
 * https://github.com/Azure-Samples/cognitive-services-speech-sdk/tree/9495674fe4f99080afe87c319d9276f6d09905e3
 */
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Transcription;
using ProjectOwl.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ProjectOwl.Services
{
    public class SpeechService : ISpeechService
    {
        public SpeechService()
        {
        }        

        /// <summary>
        /// Extract Text Content from Audio file
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<List<string>> ProcessAudio(Stream stream)
        {
            // Create the speech config object
            // Substitute real information for "YourSubscriptionKey" and "Region"
            var subkey = Environment.GetEnvironmentVariable("AzureSpeechServiceKey");
            var region = Environment.GetEnvironmentVariable("AzureSpeechServiceRegion");

            SpeechConfig speechConfig = SpeechConfig.FromSubscription(subkey, region);
            speechConfig.SetProperty("ConversationTranscriptionInRoomAndOnline", "true");

            // Set the property for asynchronous transcription
            speechConfig.SetServiceProperty("transcriptionMode", "async", ServicePropertyChannel.UriQueryParameter);

            // Create an audio stream from a wav file or from the default microphone if you want to stream live audio from the supported devices
            // Replace with your own audio file name and Helper class which implements AudioConfig using PullAudioInputStreamCallback
            PullAudioInputStreamCallback wavfilePullStreamCallback = Helper.CreateWavReader(stream);
            // Create an audio stream format assuming the file used above is 16kHz, 16 bits and 8 channel pcm wav file
            AudioStreamFormat audioStreamFormat = AudioStreamFormat.GetWaveFormatPCM(16000, 16, 8);
            // Create an input stream
            AudioInputStream audioStream = AudioInputStream.CreatePullStream(wavfilePullStreamCallback, audioStreamFormat);

            // Ensure the conversationId for a new conversation is a truly unique GUID
            String conversationId = Guid.NewGuid().ToString();

            // Create a Conversation
            using var conversation = await Conversation.CreateConversationAsync(speechConfig, conversationId);
            using var conversationTranscriber = new ConversationTranscriber(AudioConfig.FromStreamInput(audioStream));
            await conversationTranscriber.JoinConversationAsync(conversation);

            // Helper function to get the real time transcription results
            var result = await GetRecognizerResult(conversationTranscriber);

            return result; 
        }

        #region Private Methods

        /// <summary>
        /// Handle transcibing audio events
        /// </summary>
        /// <param name="recognizer"></param>
        /// <returns></returns>
        private async Task CompleteContinuousRecognition(ConversationTranscriber recognizer)
        {
            var finishedTaskCompletionSource = new TaskCompletionSource<int>();

            recognizer.SessionStopped += (s, e) =>
            {
                finishedTaskCompletionSource.TrySetResult(0);
            };
            string canceled = string.Empty;

            recognizer.Canceled += (s, e) => {
                canceled = e.ErrorDetails;
                if (e.Reason == CancellationReason.Error)
                {
                    finishedTaskCompletionSource.TrySetResult(0);
                }
            };

            await recognizer.StartTranscribingAsync().ConfigureAwait(false);
            await Task.WhenAny(finishedTaskCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(10)));
            await recognizer.StopTranscribingAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get Text Transcribed result
        /// </summary>
        /// <param name="recognizer"></param>
        /// <returns></returns>
        private async Task<List<string>> GetRecognizerResult(ConversationTranscriber recognizer)
        {
            List<string> recognizedText = new List<string>();
            recognizer.Transcribed += (s, e) =>
            {
                if (e.Result.Text.Length > 0)
                {
                    recognizedText.Add(e.Result.Text);
                }
            };

            await CompleteContinuousRecognition(recognizer);

            recognizer.Dispose();
            return recognizedText;
        }

        #endregion
    }

    #region Speech Audio Helper

    /// <summary>
    /// Helper Class for Handling Audio file
    /// </summary>
    public class Helper
    {
        public static AudioConfig OpenWavFile(string filename)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(filename));
            return OpenWavFile(reader);
        }

        public static AudioConfig OpenWavFile(BinaryReader reader)
        {
            AudioStreamFormat format = ReadWaveHeader(reader);
            return AudioConfig.FromStreamInput(new BinaryAudioStreamReader(reader), format);
        }

        public static BinaryAudioStreamReader CreateWavReader(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            // read the wave header so that it won't get into the in the following readings
            AudioStreamFormat format = ReadWaveHeader(reader);
            return new BinaryAudioStreamReader(reader);
        }

        public static AudioStreamFormat ReadWaveHeader(BinaryReader reader)
        {
            // Tag "RIFF"
            char[] data = new char[4];
            reader.Read(data, 0, 4);
            Trace.Assert((data[0] == 'R') && (data[1] == 'I') && (data[2] == 'F') && (data[3] == 'F'), "Wrong wav header");

            // Chunk size
            long fileSize = reader.ReadInt32();

            // Subchunk, Wave Header
            // Subchunk, Format
            // Tag: "WAVE"
            reader.Read(data, 0, 4);
            Trace.Assert((data[0] == 'W') && (data[1] == 'A') && (data[2] == 'V') && (data[3] == 'E'), "Wrong wav tag in wav header");

            // Tag: "fmt"
            reader.Read(data, 0, 4);
            Trace.Assert((data[0] == 'f') && (data[1] == 'm') && (data[2] == 't') && (data[3] == ' '), "Wrong format tag in wav header");

            // chunk format size
            var formatSize = reader.ReadInt32();
            var formatTag = reader.ReadUInt16();
            var channels = reader.ReadUInt16();
            var samplesPerSecond = reader.ReadUInt32();
            var avgBytesPerSec = reader.ReadUInt32();
            var blockAlign = reader.ReadUInt16();
            var bitsPerSample = reader.ReadUInt16();

            // Until now we have read 16 bytes in format, the rest is cbSize and is ignored for now.
            if (formatSize > 16)
                reader.ReadBytes((int)(formatSize - 16));

            // Second Chunk, data
            // tag: data.
            reader.Read(data, 0, 4);
            Trace.Assert((data[0] == 'd') && (data[1] == 'a') && (data[2] == 't') && (data[3] == 'a'), "Wrong data tag in wav");
            // data chunk size
            int dataSize = reader.ReadInt32();

            // now, we have the format in the format parameter and the
            // reader set to the start of the body, i.e., the raw sample data
            return AudioStreamFormat.GetWaveFormatPCM(samplesPerSecond, (byte)bitsPerSample, (byte)channels);
        }
    }

    /// <summary>
    /// Adapter class to the native stream api.
    /// </summary>
    public sealed class BinaryAudioStreamReader : PullAudioInputStreamCallback
    {
        private BinaryReader _reader;

        /// <summary>
        /// Creates and initializes an instance of BinaryAudioStreamReader.
        /// </summary>
        /// <param name="reader">The underlying stream to read the audio data from. Note: The stream contains the bare sample data, not the container (like wave header data, etc).</param>
        public BinaryAudioStreamReader(BinaryReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Creates and initializes an instance of BinaryAudioStreamReader.
        /// </summary>
        /// <param name="stream">The underlying stream to read the audio data from. Note: The stream contains the bare sample data, not the container (like wave header data, etc).</param>
        public BinaryAudioStreamReader(Stream stream)
            : this(new BinaryReader(stream))
        {
        }

        /// <summary>
        /// Reads binary data from the stream.
        /// </summary>
        /// <param name="dataBuffer">The buffer to fill</param>
        /// <param name="size">The size of data in the buffer.</param>
        /// <returns>The number of bytes filled, or 0 in case the stream hits its end and there is no more data available.
        /// If there is no data immediate available, Read() blocks until the next data becomes available.</returns>
        public override int Read(byte[] dataBuffer, uint size)
        {
            return _reader.Read(dataBuffer, 0, (int)size);
        }

        /// <summary>
        /// This method performs cleanup of resources.
        /// The Boolean parameter <paramref name="disposing"/> indicates whether the method is called from <see cref="IDisposable.Dispose"/> (if <paramref name="disposing"/> is true) or from the finalizer (if <paramref name="disposing"/> is false).
        /// Derived classes should override this method to dispose resource if needed.
        /// </summary>
        /// <param name="disposing">Flag to request disposal.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                _reader.Dispose();
            }

            disposed = true;
            base.Dispose(disposing);
        }


        private bool disposed = false;
    }

    #endregion
}
