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
    public class TextAnalyticsService : ITextAnalyticsService
    {
        private readonly HttpClient _httpClient;

        public TextAnalyticsService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("NlpApiUrl"));
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get Sentiment
        /// </summary>
        /// <param name="content"></param>
        /// <param name="authHeader"></param>
        /// <returns></returns>
        public async Task<SentimentResponse> GetSentiment(string text, AuthenticationHeaderValue authHeader)
        {
            try
            {
                var request = new DocumentRequest
                {
                    Document = new Document { Text = text }
                };

                _httpClient.DefaultRequestHeaders.Authorization = authHeader;
                var content = new StringContent(
                    JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var response = await _httpClient
                    .PostAsync("v2/analyze/standard/en/sentiment", content);
                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<SentimentResponse>(
                    await response.Content.ReadAsStringAsync());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }            
        }

        /// <summary>
        /// Get Taxonomy
        /// </summary>
        /// <param name="text"></param>
        /// <param name="authHeader"></param>
        /// <returns></returns>
        public async Task<TaxonomyResponse> GetTaxonomy(string text, AuthenticationHeaderValue authHeader)
        {
            try
            {
                var request = new DocumentRequest
                {
                    Document = new Document { Text = text }
                };

                _httpClient.DefaultRequestHeaders.Authorization = authHeader;
                var content = new StringContent(
                    JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var response = await _httpClient
                    .PostAsync("v2/categorize/emotional-traits/en", content);
                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<TaxonomyResponse>(
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
