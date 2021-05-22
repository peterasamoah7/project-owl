using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectOwl.Models
{
    public class SentimentResult
    {
        [JsonProperty("overall")]
        public decimal Overall { get; set; }

        [JsonProperty("negativity")]
        public decimal Negativity { get; set; }

        [JsonProperty("positivity")]
        public decimal Positivity { get; set; }

        [JsonProperty("items")]
        public List<SentimentItem> Items { get; set; }
    }
}
