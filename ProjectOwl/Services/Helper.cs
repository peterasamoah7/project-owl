//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// https://github.com/Azure-Samples/cognitive-services-speech-sdk/tree/master/quickstart/csharp/dotnet
//
using Microsoft.CognitiveServices.Speech.Audio;
using System.Diagnostics;
using System.IO;

namespace ProjectOwl.Services
{
    public partial class SpeechService
    {
        public class Helper
        {
            public static AudioConfig OpenWavFile(string filename)
            {
                BinaryReader reader = new BinaryReader(File.OpenRead(filename));
                return OpenWavFile(reader);
            }

            public static AudioConfig OpenWavFile(Stream fileStream)
            {
                BinaryReader reader = new BinaryReader(fileStream);
                return OpenWavFile(reader);
            }

            public static AudioConfig OpenWavFile(BinaryReader reader)
            {
                AudioStreamFormat format = readWaveHeader(reader);
                return AudioConfig.FromStreamInput(new BinaryAudioStreamReader(reader), format);
            }


            public static BinaryAudioStreamReader CreateWavReader(string filename)
            {
                BinaryReader reader = new BinaryReader(File.OpenRead(filename));
                // read the wave header so that it won't get into the in the following readings
                AudioStreamFormat format = readWaveHeader(reader);
                return new BinaryAudioStreamReader(reader);
            }

            public static AudioStreamFormat readWaveHeader(BinaryReader reader)
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
                //Trace.Assert((data[0] == 'd') && (data[1] == 'a') && (data[2] == 't') && (data[3] == 'a'), "Wrong data tag in wav");
                // data chunk size
                int dataSize = reader.ReadInt32();

                // now, we have the format in the format parameter and the
                // reader set to the start of the body, i.e., the raw sample data
                return AudioStreamFormat.GetWaveFormatPCM(samplesPerSecond, (byte)bitsPerSample, (byte)channels);
            }
        }
    }
}
