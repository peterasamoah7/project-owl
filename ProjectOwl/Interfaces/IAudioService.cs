using Microsoft.AspNetCore.Http;
using ProjectOwl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOwl.Interfaces
{
    public interface IAudioService
    {
        Task<string> AddAudioAsync(IFormFile file);
        Task<AudioModel> GetAudioAsync(string fileName);
        Task<PagedResult<List<AudioModel>>> GetPagedAudiosAsync(int pageNumber, int pageSize, Issue? issue = null, AuditStatus? status = null);
        Task<Stream> PlayAudio(string fileName);
        Task<string> ProcessAudioAsync(string message);
    }
}
