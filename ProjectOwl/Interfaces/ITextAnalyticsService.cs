using ProjectOwl.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOwl.Interfaces
{
    public interface ITextAnalyticsService
    {
        Task<SentimentResponse> GetSentiment(string text, AuthenticationHeaderValue authHeader);
        Task<TaxonomyResponse> GetTaxonomy(string text, AuthenticationHeaderValue authHeader);
    }
}
