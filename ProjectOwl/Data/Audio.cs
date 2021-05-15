using ProjectOwl.Models;
using System;

namespace ProjectOwl.Data
{
    public class Audio
    {
        /// <summary>
        /// Audio PK
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Audio file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Audio file ext: mp3, wav, etc
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Sentiment value: -20, 5, 1 etc
        /// </summary>
        public decimal Sentiment { get; set; }

        /// <summary>
        /// Related Issue
        /// </summary>
        public Issue Issue { get; set; }

        /// <summary>
        /// Transcript of Audio
        /// </summary>
        public string Transcript { get; set; }

        /// <summary>
        /// Date of audio entry
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Audio duration 
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Status of audio audit
        /// </summary>
        public AuditStatus Status { get; set; }
    }
}
