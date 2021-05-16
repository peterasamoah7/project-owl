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
        Task<AudioModel> GetAudioAsync(string fileName);
        Task<PagedResult<List<AudioModel>>> GetPagedAudiosAsync(int pageNumber, int pageSize);
        Task ProcessAudioAsync(string message);
    }
}
