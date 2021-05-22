using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProjectOwl.Models
{
    public class SentimentItem
    {
        [JsonProperty("lemma")]
        public string Lemma { get; set; }

        [JsonProperty("syncon")]
        public int Sycon { get; set; }

        [JsonProperty("sentiment")]
        public decimal Sentiment { get; set; }

        [JsonProperty("items")]
        public List<SentimentSubItem> Items { get; set; }
    }
}
