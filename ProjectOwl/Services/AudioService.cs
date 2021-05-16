using Microsoft.AspNetCore.Http;
using ProjectOwl.Data;
using ProjectOwl.Interfaces;
using ProjectOwl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOwl.Services
{
    public class AudioService : IAudioService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IBlobStorageService _blobStorageService;

        public AudioService(ApplicationDbContext dbContext, IBlobStorageService blobStorageService)
        {
            _dbContext = dbContext;
            _blobStorageService = blobStorageService; 
        }

        /// <summary>
        /// Add an audio for auditing
        /// </summary>
        /// <param name="file"></param>
        /// <param name="issue"></param>
        /// <returns></returns>
        public async Task<string> AddAudioAsync(IFormFile file, Issue issue)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            await _blobStorageService.UploadFileAsync(Container.Audio, file, fileName);

            var audio = new Audio
            {
                FileName = fileName,
                FileExtension = Path.GetExtension(file.FileName),
                Issue = issue,
                Created = DateTime.Now,
                Status = AuditStatus.Pending
            };

            await _dbContext.Audios.AddAsync(audio);
            await _dbContext.SaveChangesAsync();

            return fileName; 
        }
    }
}
