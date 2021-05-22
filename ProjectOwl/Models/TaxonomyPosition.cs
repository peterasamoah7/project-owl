using Newtonsoft.Json;

namespace ProjectOwl.Models
{
    public class TaxonomyPosition
    {
        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("end")]
        public int End { get; set; }
    }
}
