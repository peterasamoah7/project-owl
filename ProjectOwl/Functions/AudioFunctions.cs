using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using ProjectOwl.Models;
using ProjectOwl.Interfaces;
using Newtonsoft.Json;

namespace ProjectOwl.Functions
{
    public class AudioFunctions
    {
        private readonly IAudioService _audioService;

        public AudioFunctions(IAudioService audioService)
        {
            _audioService = audioService; 
        }

        [FunctionName("AddAudioFunction")]
        public async Task<IActionResult> AddAudio(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "audio")] HttpRequest req,
            [Queue(Queue.Audio), StorageAccount("AzureWebJobsStorage")] ICollector<string> outputQueueItem)
        {
            var form = await req.ReadFormAsync();

            if (!form.Files.Any())
                return new BadRequestResult(); 

            if(!Enum.TryParse<Issue>(form["issue"].ToString(), true, out var issue))
                return new BadRequestResult();

            var filename = await _audioService.AddAudioAsync(form.Files[0], issue);

            outputQueueItem.Add(JsonConvert.SerializeObject(new ProcessAudioMessage 
            {
                FileName = filename
            }));

            return new OkResult(); 
        }

        [FunctionName("ProcessAudioFunction")]
        public async Task ProcessAudio([QueueTrigger("audio", Connection = "AzureWebJobsStorage")] string myQueueItem)
        {
        }

        [FunctionName("GetAudioFunction")]
        public async Task<IActionResult> GetAudio(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "audio/{id}")] HttpRequest req, string id)
        {
            return new OkResult();
        }

        [FunctionName("GetAllAudiosFunction")]
        public async Task<IActionResult> GetAllAudios(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "audio")] HttpRequest req)
        {
            return new OkResult();
        }
    }
}
