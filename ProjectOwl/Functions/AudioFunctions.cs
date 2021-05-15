using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace ProjectOwl.Functions
{
    public class AudioFunctions
    {
        public AudioFunctions()
        {
        }

        [FunctionName("AddAudioFunction")]
        public async Task<IActionResult> AddAudio(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "audio")] HttpRequest req)
        {
            return new OkResult(); 
        }

        [FunctionName("ProcessAudioFunction")]
        public async Task ProcessAudio([QueueTrigger("audio", Connection = "")] string myQueueItem)
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "audio/{pn}/{ps}")] HttpRequest req, string pn, string ps)
        {
            return new OkResult();
        }
    }
}
