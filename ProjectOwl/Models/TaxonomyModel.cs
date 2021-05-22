using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProjectOwl.Models
{
    public class TaxonomyModel
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("categories")]
        public List<TaxonomyCategory> Categories { get; set; }
    }
}