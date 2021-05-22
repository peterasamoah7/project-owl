using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProjectOwl.Models
{
    public class SentimentModel
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("language")]
        public string Language { get; set;  }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("knowledge")]
        public List<Knowledge> Knowledge { get; set; }

        [JsonProperty("sentiment")]
        public SentimentResult Sentiment { get; set; }
    }
}
