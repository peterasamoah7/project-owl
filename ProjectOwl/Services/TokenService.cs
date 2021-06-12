using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ProjectOwl.Interfaces;
using ProjectOwl.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOwl.Services
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly string tokenKey = "auth-token"; 

        public TokenService(HttpClient httpClient, IMemoryCache cache)
        {
            httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("AuthUrl"));
            _httpClient = httpClient;
            _cache = cache; 
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
                ///check if cached token is expired; 
                if(_cache.TryGetValue<string>(tokenKey, out var value))
                {
                    var jwtToken = new JwtSecurityToken(value);
                    if (jwtToken.ValidTo > DateTime.UtcNow)
                        return new AuthenticationHeaderValue("Bearer", value);
                }

                ///create a new token if cached is expired or missing
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

                var token = await response.Content.ReadAsStringAsync();

                ///cache new token to be reused in next request
                _cache.Set(tokenKey, token); 

                return new AuthenticationHeaderValue("Bearer", token);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }            
        }
    }
}
