using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ProjectOwl.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> GenerateSasToken(string containerName);
        Task UploadFileAsync(string containerName, IFormFile file, string fileName);
    }
}
