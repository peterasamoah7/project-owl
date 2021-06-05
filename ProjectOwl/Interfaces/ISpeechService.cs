using ProjectOwl.Models;
using System.IO;
using System.Threading.Tasks;

namespace ProjectOwl.Interfaces
{
    public interface ISpeechService
    {
        Task ContinuousRecognitionWithFileAsync(Stream stream, TextCapture textCapture);
    }
}
