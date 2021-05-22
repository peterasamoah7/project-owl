using Newtonsoft.Json;

namespace ProjectOwl.Models
{
    public class SentimentResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public SentimentModel Data { get; set; }
    }
}
