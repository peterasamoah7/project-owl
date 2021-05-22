using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOwl.Services
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Get Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<T> GetModelAsync<T>(this HttpRequest req)
        {
            string requestBody = String.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }
            return JsonConvert.DeserializeObject<T>(requestBody);
        }

        /// <summary>
        /// Get Query 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetQuery(this HttpRequest req, string key)
        {
            return req.Query.TryGetValue(key, out var value) ? value.ToString() : null;
        }
    }
}
