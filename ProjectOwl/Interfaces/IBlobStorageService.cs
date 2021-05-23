using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace ProjectOwl.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> GenerateSasToken(string containerName);
        Task<Stream> GetFileAsync(string containerName, string fileName);
        Task UploadFileAsync(string containerName, IFormFile file, string fileName);
    }
}
