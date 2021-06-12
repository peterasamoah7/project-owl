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
        private readonly ITextAnalyticsService _textAnalyticsService;
        private readonly ITokenService _tokenService;
        private readonly ISpeechService _speechService; 

        public AudioService(
            ApplicationDbContext dbContext, 
            IBlobStorageService blobStorageService, 
            ITextAnalyticsService textAnalyticsService,
            ITokenService tokenService,
            ISpeechService speechService)
        {
            _dbContext = dbContext;
            _blobStorageService = blobStorageService;
            _textAnalyticsService = textAnalyticsService;
            _tokenService = tokenService;
            _speechService = speechService;
        }

        /// <summary>
        /// Add an audio for auditing
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> AddAudioAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName); 
            var fileName = $"{Guid.NewGuid()}{ext}";

            await _blobStorageService.UploadFileAsync(Container.Audio, file, fileName);

            return fileName;
        }

        /// <summary>
        /// Process message from queue
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<string> ProcessAudioAsync(string message)
        {
            var msg = JsonConvert.DeserializeObject<ProcessAudioMessage>(message);

            ///get blob 
            var blob = await _blobStorageService.GetFileAsync(Container.Audio, msg.FileName);

            ///extract text from audio file
            var capture = new TextCapture(); 
            await _speechService.ContinuousRecognitionWithFileAsync(await blob.OpenReadAsync(), capture);

            /// get auth token
            var token = await _tokenService.GetAuthTokenAsync();

            /// get overall sentiment
            var sentiment = await _textAnalyticsService.GetSentiment(capture.Text, token);
            
            /// related emotional taxonomies
            var taxonomy = await _textAnalyticsService.GetTaxonomy(capture.Text, token);
            string taxonomyStr = string
                .Join(", ", taxonomy.Data.Categories
                .Select(x => x.Label)).TrimEnd(',', ' ');      

            ///create audio entry with details from above
            var audio = new Audio
            {
                FileName = msg.FileName,
                FileExtension = Path.GetExtension(msg.FileName),
                Issue = msg.Issue,
                Created = DateTime.Now,
                Sentiment = sentiment.Data.Sentiment.Overall,
                Taxonomy = taxonomyStr,
                Status = AuditStatus.Done,
                Transcript = capture.Text
            };

            await _dbContext.Audios.AddAsync(audio);
            await _dbContext.SaveChangesAsync();

            return audio.FileName; 
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

            return new AudioModel
            {
                FileName = audio.FileName,
                Issue = audio.Issue,
                Priority = AudioHelpers.GetPriority(audio.Sentiment),
                Transcript = audio.Transcript,
                Created = audio.Created.ToString("dddd, dd MMMM yyyy"),
                Taxonomy = audio.Taxonomy?.Split(','),
                Status = audio.Status
            };
        }

        /// <summary>
        /// Get All Audios
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PagedResult<List<AudioModel>>> GetPagedAudiosAsync(
            int pageNumber, int pageSize, Issue? issue = null, AuditStatus? status = null)
        {
            var filter = new PaginationFilter(pageNumber, pageSize);
            var entries = await _dbContext.Audios
              .Skip((filter.PageNumber - 1) * filter.PageSize)
              .Take(filter.PageSize)
              .OrderBy(x => x.Created)
              .ToListAsync();

            if (issue.HasValue)
                entries = entries.Where(x => x.Issue == issue.Value).ToList();

            if (status.HasValue)
                entries = entries.Where(x => x.Status == status.Value).ToList();

            var totalRecords = await _dbContext.Audios.CountAsync();

            var audios = entries.Select(audio => new AudioModel 
            {
                FileName = audio.FileName,
                Issue = audio.Issue,
                Priority = AudioHelpers.GetPriority(audio.Sentiment),
                Transcript = audio.Transcript,
                Created = audio.Created.ToString("dddd, dd MMMM yyyy"),
                Taxonomy = audio.Taxonomy?.Split(','),
                Status = audio.Status
            })
            .ToList();

            return new PagedResult<List<AudioModel>>
                    (audios, filter.PageNumber, filter.PageSize, totalRecords);
        }

        /// <summary>
        /// Return an audio stream
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<Stream> PlayAudio(string fileName)
        {
            var audio = await _blobStorageService.GetFileAsync(Container.Audio, fileName);

            if (audio == null)
                return null; 

            return await audio.OpenReadAsync(); 
        }
    }
}
