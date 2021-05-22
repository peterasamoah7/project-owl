using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProjectOwl.Models
{
    public class TaxonomyCategory
    {
        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("hierarchy")]
        public List<string> Hierarchy { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("frequency")]
        public decimal Frequency { get; set; }

        [JsonProperty("winner")]
        public string Winner { get; set; }

        [JsonProperty("positions")]
        public List<TaxonomyPosition> Positions { get; set; }
    }
}
