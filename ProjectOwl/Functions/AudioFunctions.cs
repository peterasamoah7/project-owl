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
using ProjectOwl.Services;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace ProjectOwl.Functions
{
    public class AudioFunctions
    {
        private readonly IAudioService _audioService;

        public AudioFunctions(IAudioService audioService)
        {
            _audioService = audioService; 
        }

        /// <summary>
        /// Add Audio Function
        /// Add audio to blob and create record
        /// </summary>
        /// <param name="req"></param>
        /// <param name="outputQueueItem"></param>
        /// <returns></returns>
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

            var filename = await _audioService.AddAudioAsync(form.Files[0]);

            outputQueueItem.Add(JsonConvert.SerializeObject(new ProcessAudioMessage 
            {
                FileName = filename,
                Issue = issue
            }));

            return new OkResult(); 
        }

        /// <summary>
        /// SignalR Connector
        /// </summary>
        /// <param name="req"></param>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "notify")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        /// <summary>
        /// Process Audio Function
        /// Process audio content using AI
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [FunctionName("ProcessAudioFunction")]
        public async Task ProcessAudio([QueueTrigger(Queue.Audio, Connection = "AzureWebJobsStorage")] string msg,
            [SignalR(HubName = "notify")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            if (!string.IsNullOrEmpty(msg) || !string.IsNullOrWhiteSpace(msg))
            {
                var filename = await _audioService.ProcessAudioAsync(msg);
                await signalRMessages.AddAsync(new SignalRMessage
                {
                    Target = "notify",
                    Arguments = new[] { new NotifyModel { 
                            State = State.Done,
                            FileName = filename
                        } 
                    }
                });
            }            
        }

        /// <summary>
        /// Get Audio Function
        /// Get audio record from database
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [FunctionName("GetAudioFunction")]
        public async Task<IActionResult> GetAudio(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "audio/{id}")] HttpRequest req, string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                return new BadRequestResult();

            var response = await _audioService.GetAudioAsync(id);

            if (response == null)
                return new BadRequestResult(); 

            return new OkObjectResult(response);
        }

        /// <summary>
        /// Get All Audios Function
        /// Get audio records from database
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("GetAllAudiosFunction")]
        public async Task<IActionResult> GetAllAudios(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "audio")] HttpRequest req)
        {
            var pageNumber = req.GetQuery("pageNumber");
            var pageSize = req.GetQuery("pageSize");
            var issue = req.GetQuery("issue");
            var status = req.GetQuery("status");

            Issue? iss = null;
            AuditStatus? auditStatus = null; 
            if (!string.IsNullOrEmpty(issue))
            {
                if (Enum.TryParse<Issue>(issue, true, out var value))
                    iss = value; 
            }    
            
            if(!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<AuditStatus>(status, true, out var value)) 
                    auditStatus = value;
            }

            if (!int.TryParse(pageNumber, out var pn) || !int.TryParse(pageSize, out var ps))
                return new BadRequestResult();

            return new OkObjectResult(await _audioService.GetPagedAudiosAsync(pn, ps, iss, auditStatus));
        }

        /// <summary>
        /// Return url for streaming audio file
        /// </summary>
        /// <param name="req"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [FunctionName("AudioPlayFunction")]
        public async Task<IActionResult> PlayAudio(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "audio/play/{file}")] HttpRequest req, string file)
        {
            if (string.IsNullOrEmpty(file) || string.IsNullOrWhiteSpace(file))
                return new BadRequestResult();

            return new OkObjectResult(await _audioService.PlayAudio(file)); 
        }

        /// <summary>
        /// Add health check endpoint
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("AudioFunctionHealthCheck")]
        public static IActionResult Health(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req)
        {
            return new OkObjectResult("OK"); 
        }
    }
}
