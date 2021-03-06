using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectOwl.Models
{
    public class ProcessAudioMessage
    {
        /// <summary>
        /// Audio file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Audio Issue
        /// </summary>
        public Issue Issue { get; set; }
    }
}
