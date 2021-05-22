using Newtonsoft.Json;

namespace ProjectOwl.Models
{
    public class DocumentRequest
    {
        /// <summary>
        /// Document wrapper model
        /// </summary>
        [JsonProperty("document")]
        public Document Document { get; set; }
    }

    public class Document
    {
        /// <summary>
        /// Text content
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
