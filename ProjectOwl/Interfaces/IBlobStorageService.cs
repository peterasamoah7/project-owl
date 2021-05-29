using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace ProjectOwl.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> GenerateSasToken(string containerName);
        Task<CloudBlockBlob> GetFileAsync(string containerName, string fileName);
        Task UploadFileAsync(string containerName, IFormFile file, string fileName);
    }
}
