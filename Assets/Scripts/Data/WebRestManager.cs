using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Utility;

namespace Data
{
    public class WebRestManager
    {
        private static readonly Dictionary<string, Action<HttpRequestHeaders, string, string>> RequestHeaderSetters =
            new(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "<any>", (headers, name, value) => headers.Add(name, value)
                },
                {
                    "Authorization",
                    (headers, name, value) => headers.Authorization = AuthenticationHeaderValue.Parse("Basic " + value)
                }
            };
        
        
        public async Task<T> GetJson<T>(string endpoint)
        {
            Debug.Log($"GET {endpoint}");
            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            using var response = await new HttpClient().SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(body);
        }

        public async Task PutJson<T>(
            string endpoint, 
            T payload,
            Dictionary<string, string> headers = null)
        {
            Debug.Log($"POST {endpoint}");
            PladdraDebug.LogJson(new
            {
                endpoint,
                headers,
                payload
            });
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                "application/json");
            
            foreach (var kv in headers ?? new Dictionary<string, string>())
            {
                (RequestHeaderSetters[kv.Key] ?? RequestHeaderSetters["<any>"])(request.Headers, kv.Key, kv.Value);
            }            
            

            using var response = await new HttpClient().SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}