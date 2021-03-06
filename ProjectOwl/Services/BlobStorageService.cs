using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ProjectOwl.Interfaces;
using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace ProjectOwl.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly CloudStorageAccount _storageAccount;

        public BlobStorageService()
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            _storageAccount = CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount) ? 
                storageAccount : throw new ArgumentNullException();
        }

        /// <summary>
        /// Upload to blob
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task UploadFileAsync(string containerName, IFormFile file, string fileName)
        {
            var container = await GetBlobContainerAsync(containerName);
            var blob = container.GetBlockBlobReference(fileName);
            blob.Properties.ContentType = file.ContentType; 
            using var fileStream = file.OpenReadStream();
            await blob.UploadFromStreamAsync(fileStream);
            fileStream.Close(); 
        }

        /// <summary>
        /// Get blob file as stream
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<CloudBlockBlob> GetFileAsync(string containerName, string fileName)
        {
            var container = await GetBlobContainerAsync(containerName);
            var blob = container.GetBlockBlobReference(fileName);

            return blob; 
        }

        /// <summary>
        /// Generate SAS token for container
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task<string> GenerateSasToken(string containerName)
        {
            var container = await GetBlobContainerAsync(containerName);
            return container.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddDays(1),
                Permissions = SharedAccessBlobPermissions.Read
            });
        }

        /// <summary>
        /// Get Azure blob container
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        private async Task<CloudBlobContainer> GetBlobContainerAsync(string containerName)
        {
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var properties = await blobClient.GetServicePropertiesAsync();
            properties.DefaultServiceVersion = "2013-08-15";
            await blobClient.SetServicePropertiesAsync(properties); 
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            return container; 
        }
    }
}
