using Microsoft.AspNetCore.Http;
using ProjectOwl.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOwl.Interfaces
{
    public interface IAudioService
    {
        Task<string> AddAudioAsync(IFormFile file, Issue issue);
    }
}
