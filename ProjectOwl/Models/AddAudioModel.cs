using Microsoft.AspNetCore.Http;

namespace ProjectOwl.Models
{
    public class AddAudioModel
    {
        /// <summary>
        /// Call center issue
        /// </summary>
        public Issue Isuse { get; set; }

        /// <summary>
        /// Audio file
        /// </summary>
        public IFormFile File { get; set; }
    }
}
