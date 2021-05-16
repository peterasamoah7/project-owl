using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectOwl.Data;
using ProjectOwl.Interfaces;
using ProjectOwl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Process message from queue
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ProcessAudioAsync(string message)
        {
            var msg = JsonConvert.DeserializeObject<ProcessAudioMessage>(message);

            ///get blob 
            /// process file for text
            /// get sentiment of text

            var audio = await _dbContext.Audios
                .FirstOrDefaultAsync(a => a.FileName == msg.FileName);

            ///update audio entry with details from above

            _dbContext.Audios.Update(audio);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Get Audio details
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<AudioModel> GetAudioAsync(string fileName)
        {
            var audio = await _dbContext.Audios
                .FirstOrDefaultAsync(a => a.FileName == fileName);

            if (audio == null)
                return null;

            var sasToken = await _blobStorageService.GenerateSasToken(Container.Audio);
            var cdn = Environment.GetEnvironmentVariable("CdnEndpoint"); 

            return new AudioModel
            {
                FileName = audio.FileName,
                Issue = audio.Issue,
                Priority = AudioHelpers.GetPriority(audio.Sentiment),
                Recording = $"{cdn}/{audio.FileName}{sasToken}",
                Transcript = audio.Transcript,
                Created = audio.Created.ToString("dddd, dd MMMM yyyy")
            };
        }

        /// <summary>
        /// Get All Audios
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PagedResult<List<AudioModel>>> GetPagedAudiosAsync(int pageNumber, int pageSize)
        {
            var filter = new PaginationFilter(pageNumber, pageSize);
            var entries = await _dbContext.Audios
              .Skip((filter.PageNumber - 1) * filter.PageSize)
              .Take(filter.PageSize)
              .OrderBy(x => x.Created)
              .ToListAsync();

            var totalRecords = await _dbContext.Audios.CountAsync();

            var sasToken = await _blobStorageService.GenerateSasToken(Container.Audio);
            var cdn = Environment.GetEnvironmentVariable("CdnEndpoint");

            var audios = entries.Select(audio => new AudioModel 
            {
                FileName = audio.FileName,
                Issue = audio.Issue,
                Priority = AudioHelpers.GetPriority(audio.Sentiment),
                Recording = $"{cdn}/{audio.FileName}{sasToken}",
                Transcript = audio.Transcript,
                Created = audio.Created.ToString("dddd, dd MMMM yyyy")
            })
            .ToList();

            return new PagedResult<List<AudioModel>>
                    (audios, filter.PageNumber, filter.PageSize, totalRecords);
        }
    }
}
