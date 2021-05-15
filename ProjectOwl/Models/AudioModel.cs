namespace ProjectOwl.Models
{
    public class AudioModel
    {
        /// <summary>
        /// Related Issue
        /// </summary>
        public Issue Issue { get; set; }

        /// <summary>
        /// Priority based on Sentiment
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// Audio duration 
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Audio recording via cdn
        /// </summary>
        public string Recording { get; set; }

        /// <summary>
        /// Audio transcript
        /// </summary>
        public string Transcript { get; set; }

        /// <summary>
        /// Date audio was added
        /// </summary>
        public string Created { get; set; }
    }
}
