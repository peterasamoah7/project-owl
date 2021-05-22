using Newtonsoft.Json;

namespace ProjectOwl.Models
{
    public class SentimentSubItem
    {
        [JsonProperty("lemma")]
        public string Lemma { get; set; }

        [JsonProperty("syncon")]
        public int Sycon { get; set; }

        [JsonProperty("sentiment")]
        public decimal Sentiment { get; set; }
    }
}
