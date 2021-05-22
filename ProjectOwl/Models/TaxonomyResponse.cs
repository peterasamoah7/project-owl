using Newtonsoft.Json;

namespace ProjectOwl.Models
{
    public class TaxonomyResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public TaxonomyModel Data { get; set; }
    }
}
