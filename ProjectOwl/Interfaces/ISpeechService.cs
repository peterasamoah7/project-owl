using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOwl.Interfaces
{
    public interface ISpeechService
    {
        Task<List<string>> ProcessAudio(Stream stream);
    }
}
