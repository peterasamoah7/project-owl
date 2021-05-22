using Newtonsoft.Json;
using ProjectOwl.Interfaces;
using ProjectOwl.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOwl.Services
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;

        public TokenService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("AuthUrl"));
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get authentication token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<AuthenticationHeaderValue> GetAuthTokenAsync()
        {
            try
            {
                var request = new TokenRequest
                {
                    Username = Environment.GetEnvironmentVariable("AccountId"),
                    Password = Environment.GetEnvironmentVariable("AccountPassword"),
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var response = await _httpClient
                    .PostAsync("oauth2/token", content);
                response.EnsureSuccessStatusCode();

                return new AuthenticationHeaderValue("Bearer",
                    await response.Content.ReadAsStringAsync());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }            
        }
    }
}
