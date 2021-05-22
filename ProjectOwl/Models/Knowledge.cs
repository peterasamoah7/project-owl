using Newtonsoft.Json;

namespace ProjectOwl.Models
{
    public class Knowledge
    {
        [JsonProperty("syncon")]
        public int Sycon { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
