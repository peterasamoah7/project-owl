using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ProjectOwl.Interfaces
{
    public interface ITokenService
    {
        Task<AuthenticationHeaderValue> GetAuthTokenAsync();
    }
}
